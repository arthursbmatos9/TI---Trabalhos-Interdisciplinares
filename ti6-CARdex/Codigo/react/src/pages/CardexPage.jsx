import brands_models from '../../brands_models.json';
import { useState } from 'react';

export default function CardexPage({ goTo, capturedCars, filter, setFilter }) {
  const [currentView, setCurrentView] = useState('brands');
  const [selectedBrand, setSelectedBrand] = useState(null);

  const goBackToBrands = () => {
    setCurrentView('brands');
    setSelectedBrand(null);
  };

  const selectBrand = (brandName) => {
    setSelectedBrand(brandName);
    setCurrentView('models');
  };

  const isCarDiscovered = (marca, modelo) => {
    return capturedCars.some(car => 
      car.marca === marca && car.modelo === modelo
    );
  };

  if (currentView === 'brands') {
    return (
      <div style={styles.container}>
        <header style={styles.header}>
          <button onClick={() => goTo("home")} style={styles.headerButton}>
            ← Voltar para Home
          </button>

          <select
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            style={styles.filterSelect}
          >
            <option value="descobertos" style={styles.optionStyle}>Descobertos</option>
            <option value="todos" style={styles.optionStyle}>Todos</option>
          </select>
        </header>

        <main style={styles.mainContent}>
          {filter === "descobertos" && capturedCars.length === 0 ? (
            <p style={styles.emptyMessage}>Nenhum carro descoberto ainda.</p>
          ) : (
            <div style={styles.carsGrid}>
              {Object.keys(brands_models.brands).map((brandName) => {
                const brand = brands_models.brands[brandName];
                const discoveredModels = capturedCars.filter(car => car.marca === brandName);
                const totalModels = brand.models.length;
                const discoveredCount = discoveredModels.length;
                const isBrandComplete = discoveredCount === totalModels;

                if (filter === "descobertos" && discoveredCount === 0) {
                  return null;
                }

                return (
                  <div 
                    key={brandName} 
                    style={{
                      ...styles.brandCard,
                      ...(isBrandComplete && styles.discoveredCard)
                    }}
                    onClick={() => selectBrand(brandName)}
                  >
                    <div style={styles.imageContainer}>
                      {discoveredModels[0]?.imagem ? (
                        <img
                          src={discoveredModels[0].imagem}
                          alt={brandName}
                          style={styles.carImage}
                        />
                      ) : (
                        <div style={styles.placeholderImage}>
                          <span style={styles.placeholderText}>
                            {brandName.charAt(0)}
                          </span>
                        </div>
                      )}
                    </div>
                    <p><strong>{brandName}</strong></p>
                    <p style={styles.carModel}>
                      {discoveredCount}/{totalModels} modelos
                    </p>
                    <p style={styles.progressText}>
                      {Math.round((discoveredCount / totalModels) * 100)}% completo
                    </p>
                    {isBrandComplete && (
                      <div style={styles.checkmark}>✓</div>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </main>
      </div>
    );
  }

  if (currentView === 'models' && selectedBrand) {
    const brand = brands_models.brands[selectedBrand];

    const filteredModels = filter === "descobertos"
      ? capturedCars.filter(car => car.marca === selectedBrand)
      : brand.models.map(model => {
          const isDiscovered = isCarDiscovered(selectedBrand, model.model_name);
          const discoveredCar = capturedCars.find(car =>
            car.marca === selectedBrand && car.modelo === model.model_name
          );

          return {
            id: `${selectedBrand}-${model.model_name}`,
            marca: selectedBrand,
            modelo: model.model_name,
            imagem: discoveredCar?.imagem || null,
            discovered: isDiscovered
          };
        });

    return (
      <div style={styles.container}>
        <header style={styles.headerModels}>
          <div style={styles.headerRow}>
            <button onClick={goBackToBrands} style={styles.headerButton}>
              ← Voltar para Marcas
            </button>
            <select
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
              style={styles.filterSelect}
            >
              <option value="descobertos" style={styles.optionStyle}>Descobertos</option>
              <option value="todos" style={styles.optionStyle}>Todos</option>
            </select>
          </div>
          <h2 style={styles.brandTitle}>{selectedBrand}</h2>
        </header>

        <main style={styles.mainContent}>
          {filter === "descobertos" && filteredModels.length === 0 ? (
            <p style={styles.emptyMessage}>
              Nenhum modelo de {selectedBrand} descoberto ainda.
            </p>
          ) : (
            <div style={styles.carsGrid}>
              {filteredModels.map((carro) => {
                const isDiscovered = filter === "descobertos" || carro.discovered;

                return (
                  <div
                    key={carro.id}
                    style={{
                      ...styles.carCard,
                      ...(isDiscovered && styles.discoveredCard)
                    }}
                  >
                    <div style={styles.imageContainer}>
                      {carro.imagem ? (
                        <img
                          src={carro.imagem}
                          alt={carro.modelo}
                          style={styles.carImage}
                        />
                      ) : (
                        <div style={styles.placeholderImage}>
                          <span style={styles.placeholderText}>
                            {isDiscovered ? "✓" : "???"}
                          </span>
                        </div>
                      )}
                    </div>
                    <p><strong>{carro.modelo}</strong></p>
                    {isDiscovered && (
                      <div style={styles.checkmark}>✓</div>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </main>
      </div>
    );
  }
}

const styles = {
  container: {
    height: '100vh',
    background: 'linear-gradient(to bottom, #1e3a8a, #172554)',
    color: 'white',
    display: 'flex',
    flexDirection: 'column'
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '16px',
    background: 'rgba(0,0,0,0.2)',
    backdropFilter: 'blur(8px)',
    gap: '16px'
  },
  headerModels: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'stretch',
    padding: '16px',
    background: 'rgba(0,0,0,0.2)',
    backdropFilter: 'blur(8px)',
    gap: '8px'
  },
  headerRow: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: '16px',
    width: '100%'
  },
  headerButton: {
    background: 'rgba(255, 255, 255, 0.1)',
    color: 'white',
    padding: '8px 12px',
    borderRadius: '8px',
    border: 'none',
    cursor: 'pointer',
    whiteSpace: 'nowrap'
  },
  brandTitle: {
    margin: 0,
    textAlign: 'center',
    fontSize: '2rem',
    fontWeight: 'bold',
    marginTop: '8px',
    marginBottom: '0',
    width: '100%'
  },
  filterSelect: {
    background: 'rgba(255, 255, 255, 0.1)',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    padding: '8px',
    fontSize: '14px',
    cursor: 'pointer',
    width: '200px',
  },
  optionStyle: {
    background: '#1e3a8a',
    color: 'white',
    padding: '8px'
  },
  mainContent: {
    flex: 1,
    padding: '16px',
    overflowY: 'auto'
  },
  emptyMessage: {
    textAlign: 'center',
    color: '#bfdbfe',
    marginTop: '50px'
  },
  carsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))',
    gap: '16px'
  },
  brandCard: {
    background: 'rgba(255, 255, 255, 0.1)',
    borderRadius: '16px',
    padding: '8px',
    textAlign: 'center',
    transition: 'transform 0.2s, opacity 0.2s',
    cursor: 'pointer',
    position: 'relative'
  },
  carCard: {
    background: 'rgba(255, 255, 255, 0.1)',
    borderRadius: '16px',
    padding: '8px',
    textAlign: 'center',
    transition: 'transform 0.2s, opacity 0.2s',
    position: 'relative'
  },
  discoveredCard: {
    background: 'rgba(34, 197, 94, 0.2)',
    border: '2px solid rgba(34, 197, 94, 0.5)'
  },
  imageContainer: {
    width: '100%',
    height: '100px',
    marginBottom: '8px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center'
  },
  carImage: {
    width: '100%',
    height: '100%',
    borderRadius: '12px',
    objectFit: 'cover'
  },
  placeholderImage: {
    width: '100%',
    height: '100%',
    borderRadius: '12px',
    background: 'rgba(255, 255, 255, 0.05)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    border: '2px dashed rgba(255, 255, 255, 0.2)'
  },
  placeholderText: {
    fontSize: '24px',
    fontWeight: 'bold',
    color: 'rgba(255, 255, 255, 0.3)'
  },
  carModel: {
    fontSize: '14px',
    margin: '4px 0'
  },
  progressText: {
    fontSize: '12px',
    color: '#93c5fd',
    opacity: 0.8
  },
  checkmark: {
    position: 'absolute',
    top: '8px',
    right: '8px',
    background: '#22c55e',
    color: 'white',
    borderRadius: '50%',
    width: '20px',
    height: '20px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: '12px',
    fontWeight: 'bold'
  }
};