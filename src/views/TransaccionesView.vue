<template>
  <div>
    <Navbar />
    <div style="max-width: 800px; margin: 0 auto; padding: 0 20px;">

      <h2>Nueva transacción</h2>

      <div style="background: #1a1a2e; padding: 20px; border-radius: 10px; margin-bottom: 30px;">
        <p v-if="error"   style="color: salmon; font-size: 14px;">{{ error }}</p>
        <p v-if="mensaje" style="color: #51cf66; font-size: 14px;">{{ mensaje }}</p>

        <!-- Compra / Venta -->
        <div style="display: flex; gap: 10px; margin-bottom: 16px;">
          <button @click="tipo = 'Compra'"
            :style="{ flex: 1, padding: '10px', borderRadius: '6px', border: 'none', cursor: 'pointer',
              background: tipo === 'Compra' ? '#51cf66' : '#333', color: tipo === 'Compra' ? '#111' : '#eee', fontWeight: 'bold' }">
            ▲ Compra
          </button>
          <button @click="tipo = 'Venta'"
            :style="{ flex: 1, padding: '10px', borderRadius: '6px', border: 'none', cursor: 'pointer',
              background: tipo === 'Venta' ? '#ff6b6b' : '#333', color: tipo === 'Venta' ? '#111' : '#eee', fontWeight: 'bold' }">
            ▼ Venta
          </button>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-bottom: 16px;">
          <div>
            <label style="font-size: 13px; color: #aaa;">Símbolo</label>
            <input v-model="simbolo" placeholder="BTC, ETH, USDT..."
              @input="simbolo = simbolo.toUpperCase()"
              style="width: 100%; padding: 10px; margin-top: 4px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
          </div>
          <div>
            <label style="font-size: 13px; color: #aaa;">Cantidad</label>
            <input v-model.number="cantidad" type="number" placeholder="0.01"
              style="width: 100%; padding: 10px; margin-top: 4px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
          </div>
          <div>
            <label style="font-size: 13px; color: #aaa;">Precio USD <span style="color: #555;">(opcional)</span></label>
            <input v-model.number="precio" type="number" placeholder="auto"
              style="width: 100%; padding: 10px; margin-top: 4px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
          </div>
        </div>

        <button @click="registrar"
          :style="{ width: '100%', padding: '12px', borderRadius: '6px', border: 'none', cursor: 'pointer', fontWeight: 'bold',
            background: tipo === 'Compra' ? '#51cf66' : '#ff6b6b', color: '#111' }">
          Registrar {{ tipo }}
        </button>
      </div>

      <!-- Historial -->
      <h2>Historial</h2>

      <div v-if="cargando" style="color: #aaa;">Cargando...</div>

      <table v-else-if="transacciones.length" style="width: 100%; border-collapse: collapse;">
        <thead>
          <tr style="border-bottom: 1px solid #333;">
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Cripto</th>
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Tipo</th>
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Cantidad</th>
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Precio</th>
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Total</th>
            <th style="padding: 10px;"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in transacciones" :key="t.id" style="border-bottom: 1px solid #222;">
            <td style="padding: 10px; font-weight: bold;">{{ t.simboloCripto }}</td>
            <td style="padding: 10px;"
              :style="{ color: t.tipo === 'Compra' ? '#51cf66' : '#ff6b6b' }">
              {{ t.tipo }}
            </td>
            <td style="padding: 10px;">{{ t.cantidad }}</td>
            <td style="padding: 10px;">${{ t.precioUsd }}</td>
            <td style="padding: 10px;">${{ t.total }}</td>
            <td style="padding: 10px;">
              <button @click="eliminar(t.id)"
                style="background: none; border: none; color: #ff6b6b; cursor: pointer; font-size: 13px;">
                Eliminar
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <p v-else style="color: #aaa;">Todavía no hay transacciones.</p>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import Navbar from '../components/Navbar.vue'

const usuario = JSON.parse(localStorage.getItem('usuario') || '{}')

const tipo     = ref('Compra')
const simbolo  = ref('')
const cantidad = ref(null)
const precio   = ref(null)
const error    = ref('')
const mensaje  = ref('')

const transacciones = ref([])
const cargando      = ref(true)

async function cargarHistorial() {
  try {
    const respuesta = await axios.get(`http://localhost:5080/api/transacciones/${usuario.id}`)
    transacciones.value = respuesta.data
  } catch {
    // si falla queda vacío
  } finally {
    cargando.value = false
  }
}

async function registrar() {
  error.value   = ''
  mensaje.value = ''

  if (!simbolo.value || !cantidad.value) {
    error.value = 'Completá el símbolo y la cantidad.'
    return
  }

  try {
    await axios.post('http://localhost:5080/api/transacciones', {
      usuarioId:     usuario.id,
      simboloCripto: simbolo.value,
      tipo:          tipo.value,
      cantidad:      cantidad.value,
      precioUsd:     precio.value || 0
    })

    mensaje.value = `${tipo.value} registrada correctamente.`
    simbolo.value  = ''
    cantidad.value = null
    precio.value   = null

    await cargarHistorial()
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al registrar.'
  }
}

async function eliminar(id) {
  if (!confirm('¿Eliminar esta transacción?')) return
  await axios.delete(`http://localhost:5080/api/transacciones/${id}`)
  await cargarHistorial()
}

onMounted(cargarHistorial)
</script>