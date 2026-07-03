<template>
  <div>
    <Navbar />
    <div style="max-width: 800px; margin: 0 auto; padding: 0 20px;">

      <h2>Mi Portafolio</h2>

      <div v-if="cargando" style="color: #aaa;">Cargando...</div>

      <div v-else>
        <div style="display: flex; gap: 16px; margin-bottom: 24px;">
          <div style="flex: 1; background: #1a1a2e; padding: 16px; border-radius: 10px;">
            <div style="font-size: 12px; color: #aaa;">Costo total</div>
            <div style="font-size: 20px; font-weight: bold;">${{ portafolio.costoTotal }}</div>
          </div>
          <div style="flex: 1; background: #1a1a2e; padding: 16px; border-radius: 10px;">
            <div style="font-size: 12px; color: #aaa;">Valor actual</div>
            <div style="font-size: 20px; font-weight: bold;">${{ portafolio.valorTotal }}</div>
          </div>
          <div style="flex: 1; background: #1a1a2e; padding: 16px; border-radius: 10px;">
            <div style="font-size: 12px; color: #aaa;">Ganancia / Pérdida</div>
            <div style="font-size: 20px; font-weight: bold;"
              :style="{ color: portafolio.gananciaPerdidaTotal >= 0 ? '#51cf66' : '#ff6b6b' }">
              ${{ portafolio.gananciaPerdidaTotal }}
            </div>
          </div>
        </div>

        <div v-if="portafolio.tenencias && portafolio.tenencias.length > 0">
          <table style="width: 100%; border-collapse: collapse;">
            <thead>
              <tr style="border-bottom: 1px solid #333;">
                <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Cripto</th>
                <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Cantidad</th>
                <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Precio actual</th>
                <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Valor actual</th>
                <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">G/P</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="t in portafolio.tenencias" :key="t.simbolo" style="border-bottom: 1px solid #222;">
                <td style="padding: 10px; font-weight: bold;">{{ t.simbolo }}</td>
                <td style="padding: 10px;">{{ t.cantidad }}</td>
                <td style="padding: 10px;">${{ t.precioActual }}</td>
                <td style="padding: 10px;">${{ t.valorActual }}</td>
                <td style="padding: 10px;" :style="{ color: t.gananciaPerdida >= 0 ? '#51cf66' : '#ff6b6b' }">
                  ${{ t.gananciaPerdida }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <p v-else style="color: #aaa;">
          No tenés criptos todavía. Andá a
          <RouterLink to="/transacciones" style="color: #4dabf7;">Transacciones</RouterLink>
          para cargar tu primera compra.
        </p>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import Navbar from '../components/Navbar.vue'

const portafolio = ref({ tenencias: [], costoTotal: 0, valorTotal: 0, gananciaPerdidaTotal: 0 })
const cargando = ref(true)

onMounted(async () => {
  const usuario = JSON.parse(localStorage.getItem('usuario') || '{}')
  try {
    const respuesta = await axios.get(`http://localhost:5080/api/portafolio/${usuario.id}`)
    portafolio.value = respuesta.data
    console.log(respuesta.data)
  } catch {
    // si falla queda el portafolio vacío
  } finally {
    cargando.value = false
  }
})
</script>