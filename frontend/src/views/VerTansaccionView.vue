<template>
  <div>
    <Navbar />
    <div style="max-width: 500px; margin: 0 auto; padding: 0 20px;">

      <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 24px;">
        <button @click="$router.push('/transacciones')"
          style="background: none; border: 1px solid #333; color: #aaa; padding: 6px 12px; border-radius: 6px; cursor: pointer;">
          ← Volver
        </button>
        <h2 style="margin: 0;">Ver transacción</h2>
      </div>

      <div v-if="cargando" style="color: #aaa;">Cargando...</div>

      <div v-else-if="transaccion" style="background: #1a1a2e; padding: 24px; border-radius: 10px;">

        <div style="margin-bottom: 16px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Criptomoneda</div>
          <div style="font-size: 18px; font-weight: bold;">{{ transaccion.simboloCripto }}</div>
        </div>

        <div style="margin-bottom: 16px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Tipo</div>
          <div :style="{ fontSize: '16px', fontWeight: 'bold', color: transaccion.tipo === 'Compra' ? '#51cf66' : '#ff6b6b' }">
            {{ transaccion.tipo }}
          </div>
        </div>

        <div style="margin-bottom: 16px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Cantidad</div>
          <div style="font-size: 16px;">{{ transaccion.cantidad }}</div>
        </div>

        <div style="margin-bottom: 16px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Precio ARS</div>
          <div style="font-size: 16px;">${{ transaccion.precioArs }}</div>
        </div>

        <div style="margin-bottom: 16px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Total ARS</div>
          <div style="font-size: 16px; font-weight: bold;">${{ transaccion.total }}</div>
        </div>

        <div style="margin-bottom: 24px;">
          <div style="font-size: 12px; color: #aaa; margin-bottom: 4px;">Fecha y hora</div>
          <div style="font-size: 16px;">{{ formatearFecha(transaccion.fecha) }}</div>
        </div>

        <button @click="$router.push(`/transacciones/${transaccion.id}/editar`)"
          style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #ffd43b; background: none; color: #ffd43b; font-weight: bold; cursor: pointer;">
          Editar esta transacción
        </button>

      </div>

      <p v-else style="color: #aaa;">No se encontró la transacción.</p>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import Navbar from '../components/Navbar.vue'

const route = useRoute()
const transaccion = ref(null)
const cargando = ref(true)

function formatearFecha(f) {
  return new Date(f).toLocaleString('es-AR', { dateStyle: 'short', timeStyle: 'short' })
}

onMounted(async () => {
  try {
    const respuesta = await axios.get(`http://localhost:5080/api/transacciones/${route.params.id}`)
    transaccion.value = respuesta.data
  } catch {
    transaccion.value = null
  } finally {
    cargando.value = false
  }
})
</script>