import { getToken } from './authService';

const API_URL = 'http://localhost:3001/api';

export const addCarToCollection = async (carBrand, carModel, imageBase64) => {
  try {
    const token = getToken();
    
    if (!token) {
      throw new Error('Usuário não autenticado');
    }

    const blob = base64ToBlob(imageBase64);
    
    const formData = new FormData();
    formData.append('carBrand', carBrand);
    formData.append('carModel', carModel);
    formData.append('image', blob, 'car.jpg');

    const response = await fetch(`${API_URL}/cars`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
      body: formData,
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao adicionar carro');
    }

    return data;
  } catch (error) {
    throw error;
  }
};

export const getUserCollection = async () => {
  try {
    const token = getToken();
    
    if (!token) {
      throw new Error('Usuário não autenticado');
    }

    const response = await fetch(`${API_URL}/cars`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao buscar coleção');
    }

    return data;
  } catch (error) {
    throw error;
  }
};

export const deleteCarFromCollection = async (carId) => {
  try {
    const token = getToken();
    
    if (!token) {
      throw new Error('Usuário não autenticado');
    }

    const response = await fetch(`${API_URL}/cars/${carId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao deletar carro');
    }

    return data;
  } catch (error) {
    throw error;
  }
};

export const getCar = async (carId) => {
  try {
    const token = getToken();
    
    if (!token) {
      throw new Error('Usuário não autenticado');
    }

    const response = await fetch(`${API_URL}/cars/${carId}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao buscar carro');
    }

    return data;
  } catch (error) {
    throw error;
  }
};

function base64ToBlob(base64) {
  const arr = base64.split(',');
  const mime = arr[0].match(/:(.*?);/)[1];
  const bstr = atob(arr[1]);
  let n = bstr.length;
  const u8arr = new Uint8Array(n);
  while (n--) {
    u8arr[n] = bstr.charCodeAt(n);
  }
  return new Blob([u8arr], { type: mime });
}
