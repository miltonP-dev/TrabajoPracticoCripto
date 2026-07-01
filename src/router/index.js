import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegistroView from '../views/RegistroView.vue'
import DashboardView from '../views/DashboardView.vue'
import TransaccionesView from '../views/TransaccionesView.vue'

const routes = [
  { path: '/',              redirect: '/login' },
  { path: '/login',         component: LoginView },
  { path: '/registro',      component: RegistroView },
  { path: '/dashboard',     component: DashboardView },
  { path: '/transacciones', component: TransaccionesView }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Si no hay usuario guardado, redirige al login
router.beforeEach((to) => {
  const usuario = localStorage.getItem('usuario')
  const rutasPublicas = ['/login', '/registro']
  if (!usuario && !rutasPublicas.includes(to.path)) {
    return '/login'
  }
})

export default router