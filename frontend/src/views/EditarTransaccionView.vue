<template>
  <div>
    <Navbar />
    <div style="max-width: 500px; margin: 0 auto; padding: 0 20px;">

      <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 24px;">
        <button @click="$router.push('/transacciones')"
          style="background: none; border: 1px solid #333; color: #aaa; padding: 6px 12px; border-radius: 6px; cursor: pointer;">
          ← Volver
        </button>
        <h2 style="margin: 0;">Editar transacción</h2>
      </div>

      <div v-if="cargando" style="color: #aaa;">Cargando...</div>

      <div v-else style="background: #1a1a2e; padding: 24px; border-radius: 10px;">

        <p v-if="error"   style="color: salmon; font-size: 14px;">{{ error }}</p>
        <p v-if="mensaje" style="color: #51cf66; font-size: 14px;">{{ mensaje }}</p>

        <!-- Compra / Venta -->
        <div style="display: flex; gap: 10px; margin-bottom: 16px;">
          <button @click="form.tipo = 'Compra'"
            :style="{ flex: 1, padding: '10px', borderRadius: '6px', border: 'none', cursor: 'pointer',
              background: form.tipo === 'Compra' ? '#51cf66' : '#333', color: form.tipo === 'Compra' ? '#111' : '#eee', fontWeight: 'bold' }">
            ▲ Compra
          </button>
          <button @click="form.tipo = 'Venta'"
            :style="{ flex: 1, padding: '10px', borderRadius: '6px', border: 'none', cursor: 'pointer',
              background: form.tipo === 'Venta' ? '#ff6b6b' : '#333', color: form.tipo === 'Venta' ? '#111' : '#eee', fontWeight: 'bold' }">
            ▼ Venta
          </button>
        </div>

        <div style="margin-bottom: 14px;">
          <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Criptomoneda</label>
          <select v-model="form.simboloCripto"
            style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;">
            <option value="BTC">Bitcoin (BTC)</option>
            <option value="ETH">Ethereum (ETH)</option>
            <option value="USDC">USD Coin (USDC)</option>
            <option value="SOL">Solana (SOL)</option>
            <option value="ADA">Cardano (ADA)</option>
          </select>
        </div>

        <div style="margin-bottom: 14px;">
          <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Cantidad</label>
          <input v-model.number="form.cantidad" type="number" step="any" min="0"
            style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
        </div>

        <div style="margin-bottom: 14px;">
          <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Precio ARS</label>
          <input v-model.number="form.precioArs" type="number" step="any" min="0"
            style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
        </div>

        <div style="margin-bottom: 20px;">
          <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Fecha y hora</label>
          <input v-model="form.fecha" type="datetime-local"
            style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
        </div>

        <button @click="guardar"
          style="width: 100%; padding: 12px; border-radius: 6px; border: none; background: #4dabf7; color: #111; font-weight: bold; cursor: pointer;">
          Guardar cambios
        </button>

      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import Navbar from '../components/Navbar.vue'

const route  = useRoute()
const router = useRouter()

const cargando = ref(true)
const error    = ref('')
const mensaje  = ref('')

const form = reactive({
  tipo:          'Compra',
  simboloCripto: 'BTC',
  cantidad:      0,
  precioArs:     0,
  fecha:         ''
})

onMounted(async () => {
  try {
    const respuesta = await axios.get(`http://localhost:5080/api/transacciones/${route.params.id}`)
    const t = respuesta.data
    form.tipo          = t.tipo
    form.simboloCripto = t.simboloCripto
    form.cantidad      = t.cantidad
    form.precioArs     = t.precioArs
    form.fecha         = new Date(t.fecha).toISOString().slice(0, 16)
  } catch {
    error.value = 'No se pudo cargar la transacción.'
  } finally {
    cargando.value = false
  }
})

async function guardar() {
  error.value   = ''
  mensaje.value = ''

  if (!form.cantidad || form.cantidad <= 0) {
    error.value = 'La cantidad debe ser mayor a 0.'
    return
  }

  try {
    await axios.patch(`http://localhost:5080/api/transacciones/${route.params.id}`, {
      tipo:          form.tipo,
      simboloCripto: form.simboloCripto,
      cantidad:      form.cantidad,
      precioArs:     form.precioArs,
      fecha:         form.fecha
    })
    mensaje.value = 'Transacción actualizada correctamente.'
    setTimeout(() => router.push('/transacciones'), 1500)
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al guardar.'
  }
}
</script>