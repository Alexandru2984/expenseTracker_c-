import axios from 'axios'

const api = axios.create({
  baseURL: '/api'
})

export const subscriptionsApi = {
  getAll: () => api.get('/subscriptions'),
  getById: (id) => api.get(`/subscriptions/${id}`),
  create: (data) => api.post('/subscriptions', data),
  update: (id, data) => api.put(`/subscriptions/${id}`, data),
  remove: (id) => api.delete(`/subscriptions/${id}`),
  getSummary: () => api.get('/subscriptions/summary')
}
