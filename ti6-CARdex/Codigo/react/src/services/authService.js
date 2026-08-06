const API_URL = 'http://localhost:3001/api';

export const saveToken = (token) => {
  localStorage.setItem('cardex_token', token);
};

export const getToken = () => {
  return localStorage.getItem('cardex_token');
};

export const removeToken = () => {
  localStorage.removeItem('cardex_token');
};

export const saveUser = (user) => {
  localStorage.setItem('cardex_user', JSON.stringify(user));
};

export const getUser = () => {
  const user = localStorage.getItem('cardex_user');
  return user ? JSON.parse(user) : null;
};

export const removeUser = () => {
  localStorage.removeItem('cardex_user');
};

export const isAuthenticated = () => {
  return !!getToken();
};

export const register = async (username, email, password) => {
  try {
    const response = await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, email, password }),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao cadastrar');
    }

    saveToken(data.token);
    saveUser(data.user);

    return data;
  } catch (error) {
    throw error;
  }
};

export const login = async (email, password) => {
  try {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao fazer login');
    }

    saveToken(data.token);
    saveUser(data.user);

    return data;
  } catch (error) {
    throw error;
  }
};

export const logout = () => {
  removeToken();
  removeUser();
};

export const getProfile = async () => {
  try {
    const token = getToken();
    
    if (!token) {
      throw new Error('Token não encontrado');
    }

    const response = await fetch(`${API_URL}/auth/profile`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Erro ao buscar perfil');
    }

    return data.user;
  } catch (error) {
    throw error;
  }
};
