import { useState, useEffect } from "react";


export default function ResultPage({ goTo, currentBatch = [] }) {
  const [showResult, setShowResult] = useState(false);
  const [shake, setShake] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);

  const lastCars = currentBatch.length > 0 ? currentBatch : [];
  const currentCar = lastCars[currentIndex];

  useEffect(() => {
    setShowResult(false);
    setShake(false);
    setCurrentIndex(0);

    const shakeTimer = setTimeout(() => setShake(true), 500);
    const resultTimer = setTimeout(() => setShowResult(true), 2000);
    
    return () => {
      clearTimeout(shakeTimer);
      clearTimeout(resultTimer);
    };
  }, [currentBatch]);

  if (!currentCar) {
    return (
      <div style={styles.emptyContainer}>
        <p>Nenhum carro identificado.</p>
        <button onClick={() => goTo("home")} style={styles.button}>
          Voltar
        </button>
      </div>
    );
  }

  const handlePrevious = () => {
    setCurrentIndex((prev) => (prev > 0 ? prev - 1 : lastCars.length - 1));
  };

  const handleNext = () => {
    setCurrentIndex((prev) => (prev < lastCars.length - 1 ? prev + 1 : 0));
  };

  return (
    <div style={styles.container}>
      {!showResult ? (
        <div className="capture-animation">
          <div className="energy-aura"></div>
          <div className="pulse-circle"></div>
          <div
            className={
              "capture-ball" +
              (shake ? " shake capture-ball-glow" : "")
            }
          >
            <div className="ball-highlight"></div>
            <div className="ball-sparkle"></div>
          </div>
          <p className="capturing-text">
            <span className="capturing-text-anim">
              Capturando {lastCars.length} carro{lastCars.length !== 1 ? 's' : ''}...
            </span>
          </p>
        </div>
      ) : (
        <>
          <h1 style={styles.title}>Carros Identificados!</h1>

          {lastCars.length > 1 && (
            <p style={styles.counter}>
              {currentIndex + 1} de {lastCars.length}
            </p>
          )}

          {currentCar.imagem && (
            <img
              src={currentCar.imagem}
              alt="Carro capturado"
              style={styles.carImage}
            />
          )}

          <div style={styles.carInfo}>
            <p><strong>Marca:</strong> {currentCar.marca || "Desconhecido"}</p>
            <p><strong>Modelo:</strong> {currentCar.modelo || "Desconhecido"}</p>
            {"confianca" in currentCar && (
              <p>
                <strong>Confiança:</strong>{" "}
                {typeof currentCar.confianca === "number"
                  ? currentCar.confianca.toFixed(2) + "%"
                  : "Desconhecido"}
              </p>
            )}
          </div>

          {lastCars.length > 1 && (
            <div style={styles.navigationContainer}>
              <button onClick={handlePrevious} style={styles.navButton}>
                ← Anterior
              </button>
              <div style={styles.thumbnailContainer}>
                {lastCars.map((car, idx) => (
                  <button
                    key={idx}
                    onClick={() => setCurrentIndex(idx)}
                    style={{
                      ...styles.thumbnail,
                      border: idx === currentIndex ? '3px solid #3b82f6' : '2px solid rgba(255,255,255,0.3)',
                      opacity: idx === currentIndex ? 1 : 0.6
                    }}
                  >
                    {car.imagem ? (
                      <img src={car.imagem} alt={`Carro ${idx + 1}`} style={styles.thumbnailImage} />
                    ) : (
                      <div style={styles.thumbnailPlaceholder}>?</div>
                    )}
                  </button>
                ))}
              </div>
              <button onClick={handleNext} style={styles.navButton}>
                Próximo →
              </button>
            </div>
          )}

          <button onClick={() => goTo("home")} style={styles.button}>
            Capturar Mais
          </button>
        </>
      )}
    </div>
  );
}

const styles = {
  container: {
    height: '100vh',
    background: 'linear-gradient(to bottom, #1e3a8a, #172554)',
    color: 'white',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    padding: '24px',
    overflowY: 'auto'
  },
  emptyContainer: {
    height: '100vh',
    background: 'linear-gradient(to bottom, #1e3a8a, #172554)',
    color: 'white',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center'
  },
  title: {
    fontSize: '28px',
    fontWeight: 'bold',
    marginBottom: '8px'
  },
  counter: {
    fontSize: '14px',
    color: '#bfdbfe',
    marginBottom: '16px'
  },
  carImage: {
    width: '100%',
    maxWidth: '320px',
    borderRadius: '12px',
    marginBottom: '16px'
  },
  carInfo: {
    background: 'rgba(255, 255, 255, 0.1)',
    borderRadius: '16px',
    padding: '16px',
    textAlign: 'center',
    width: '100%',
    maxWidth: '320px',
    marginBottom: '24px'
  },
  navigationContainer: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '12px',
    marginBottom: '24px',
    width: '100%',
    maxWidth: '500px'
  },
  navButton: {
    background: 'rgba(255, 255, 255, 0.1)',
    color: 'white',
    border: '1px solid rgba(255, 255, 255, 0.2)',
    borderRadius: '9999px',
    padding: '8px 12px',
    cursor: 'pointer',
    fontSize: '14px',
    transition: 'all 0.2s'
  },
  thumbnailContainer: {
    display: 'flex',
    gap: '8px',
    overflowX: 'auto',
    padding: '8px 0',
    maxWidth: '300px'
  },
  thumbnail: {
    width: '50px',
    height: '50px',
    borderRadius: '8px',
    padding: 0,
    cursor: 'pointer',
    background: 'rgba(255, 255, 255, 0.1)',
    flexShrink: 0,
    transition: 'all 0.2s'
  },
  thumbnailImage: {
    width: '100%',
    height: '100%',
    borderRadius: '6px',
    objectFit: 'cover'
  },
  thumbnailPlaceholder: {
    width: '100%',
    height: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    color: 'rgba(255, 255, 255, 0.5)',
    fontSize: '20px',
    fontWeight: 'bold'
  },
  button: {
    background: 'linear-gradient(to right, #3b82f6, #2563eb)',
    color: 'white',
    fontWeight: '600',
    padding: '12px 24px',
    borderRadius: '9999px',
    border: 'none',
    cursor: 'pointer',
    fontSize: '16px',
    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
    transition: 'transform 0.2s'
  }
};
