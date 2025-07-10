import axios from 'axios';

const API = axios.create({
  baseURL: 'https://localhost:7261/api', // adresa backend
});

export default API;
