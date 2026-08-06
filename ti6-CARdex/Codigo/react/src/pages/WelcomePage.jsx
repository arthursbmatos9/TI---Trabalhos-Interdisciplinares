export default function WelcomePage({ goTo }) {
  return (
    <div style={styles.container}>
      <div style={styles.overlay}></div>

      <div style={styles.content}>
        <h1 style={styles.title}>Cardex</h1>
        <p style={styles.subtitle}>
          Descubra e capture todos os carros que puder!
        </p>
        
        <div style={styles.buttonGroup}>
          <button
            onClick={() => goTo("login")}
            style={styles.loginButton}
          >
            Entrar
          </button>
          <button
            onClick={() => goTo("register")}
            style={styles.registerButton}
          >
            Criar Conta
          </button>
        </div>
      </div>
    </div>
  );
}

const styles = {
  container: {
    height: '100vh',
    backgroundImage: 'url(/fundoMapa.png)',
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    backgroundRepeat: 'no-repeat',
    position: 'relative',
    color: 'white',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '24px'
  },
  overlay: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    background: 'rgba(0, 0, 0, 0.3)',
    zIndex: 0
  },
  content: {
    textAlign: 'center',
    position: 'relative',
    zIndex: 1
  },
  title: {
    fontSize: '36px',
    fontWeight: 'bold',
    marginBottom: '16px'
  },
  subtitle: {
    fontSize: '18px',
    color: '#bfdbfe',
    maxWidth: '300px',
    marginBottom: '32px'
  },
  buttonGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
    width: '100%',
    maxWidth: '280px'
  },
  loginButton: {
    background: 'white',
    color: '#1e3a8a',
    fontWeight: '600',
    padding: '12px 32px',
    borderRadius: '9999px',
    border: 'none',
    cursor: 'pointer',
    fontSize: '16px',
    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
    transition: 'all 0.2s'
  },
  registerButton: {
    background: 'transparent',
    color: 'white',
    fontWeight: '600',
    padding: '12px 32px',
    borderRadius: '9999px',
    border: '2px solid white',
    cursor: 'pointer',
    fontSize: '16px',
    transition: 'all 0.2s'
  }
};