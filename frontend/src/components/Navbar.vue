<template>
  <div style="display: flex; justify-content: space-between; align-items: center; padding: 12px 20px; background: #1a1a2e; margin-bottom: 20px;">
    <div>
      <RouterLink to="/dashboard"     style="margin-right: 16px; color: #4dabf7; text-decoration: none;">Dashboard</RouterLink>
      <RouterLink to="/transacciones" style="color: #4dabf7; text-decoration: none;">Transacciones</RouterLink>
    </div>
    <div style="display: flex; align-items: center; gap: 12px;">
      <span style="color: #aaa; font-size: 14px;">{{ nombreUsuario }}</span>
      <button @click="salir" style="padding: 6px 12px; cursor: pointer;">Salir</button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()
const usuario = JSON.parse(localStorage.getItem('usuario') || '{}')
const nombreUsuario = usuario.nombreUsuario || ''

function salir() {
  localStorage.removeItem('usuario')
  router.push('/login')
}
</script>