function base64ToBlob(base64) {
  const arr = base64.split(",");
  const mime = arr[0].match(/:(.*?);/)[1];
  const bstr = atob(arr[1]);
  let n = bstr.length;
  const u8arr = new Uint8Array(n);
  while (n--) {
    u8arr[n] = bstr.charCodeAt(n);
  }
  return new Blob([u8arr], { type: mime });
}

function processPrediction(pred, imageBase64, index) {
  return {
    id: Date.now().toString() + "_" + index,
    marca: pred.brand || pred.marca || "Desconhecido",
    modelo: pred.model || pred.modelo || "Desconhecido",
    ano: pred.year || pred.ano || "",
    imagem: imageBase64 || null,
    confianca: typeof pred.confidence === "number" 
      ? pred.confidence 
      : (typeof pred.confidence_percent === "number" 
        ? pred.confidence_percent 
        : 0),
  };
}

export async function identifyCar(images) {
  console.log("Processando imagens...");

  const imagesArray = Array.isArray(images) ? images : [images];
  
  if (imagesArray.length === 0) {
    throw new Error("Nenhuma imagem fornecida");
  }

  if (imagesArray.length === 1) {
    return identifyCarSingle(imagesArray[0]);
  }

  return identifyCarBatch(imagesArray);
}

async function identifyCarSingle(imageBase64) {
  console.log("Imagem recebida:", (imageBase64 || "").substring(0, 30) + "...");

  const blob = base64ToBlob(imageBase64);
  const formData = new FormData();
  formData.append("file", blob, "car.png");

  const response = await fetch("https://cardexbackend.onrender.com/predict", {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    throw new Error("Falha na requisição da API (single)");
  }

  let result;
  try {
    result = await response.json();
    console.log("Resposta da API (single):", result);
  } catch (e) {
    throw new Error("Erro ao fazer parse do JSON da resposta");
  }

  if (!result.success || !result.predictions || result.predictions.length === 0) {
    throw new Error("Nenhuma predição retornada");
  }

  return processPrediction(result.predictions[0], imageBase64, 0);
}

async function identifyCarBatch(imagesBase64Array) {
  console.log(`Processando ${imagesBase64Array.length} imagens em batch`);

  const formData = new FormData();

  imagesBase64Array.forEach((img, idx) => {
    const blob = base64ToBlob(img);
    formData.append("files", blob, `car_${idx}.png`);
  });

  const response = await fetch("https://cardexbackend.onrender.com/predict_batch", {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Erro na requisição batch:", response.status, errorText);
    throw new Error(`Erro na requisição batch da API: ${response.status}`);
  }

  let result;
  try {
    result = await response.json();
    console.log("Resposta do batch (bruta):", result);
  } catch (e) {
    throw new Error("Erro ao fazer parse da resposta batch");
  }

  if (!result.success) {
    throw new Error("Batch não foi bem-sucedido");
  }

  const results = result.results || result.predictions || [];

  if (!results || results.length === 0) {
    throw new Error("Nenhuma predição retornada no batch");
  }

  console.log(`📊 Batch contém ${results.length} resultado(s)`);

  return results.map((resultItem, idx) => {
    let bestPrediction = null;

    if (resultItem.predictions && Array.isArray(resultItem.predictions) && resultItem.predictions.length > 0) {
      bestPrediction = resultItem.predictions[0];
    }
    else if (resultItem.success && resultItem.predictions) {
      bestPrediction = resultItem.predictions[0];
    }

    if (!bestPrediction) {
      console.warn(`⚠️ Nenhuma predição válida para imagem ${idx}`);
      return {
        id: Date.now().toString() + "_" + idx,
        marca: "Desconhecido",
        modelo: "Desconhecido",
        ano: "",
        imagem: imagesBase64Array[idx] || null,
        confianca: 0,
      };
    }

    return processPrediction(bestPrediction, imagesBase64Array[idx], idx);
  });
}