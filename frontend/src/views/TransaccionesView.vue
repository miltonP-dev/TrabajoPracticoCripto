<<template>
  <div>
    <Navbar />
    <div style="max-width: 800px; margin: 0 auto; padding: 0 20px;">

      <h2>Nueva <transacción</h2>

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

          <!-- Select criptomonedas -->
          <div>
            <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Criptomoneda</label>
            <select v-model="simbolo"
              style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;">
              <option value="">Seleccioná una cripto</option>
              <option value="BTC">Bitcoin (BTC)</option>
              <option value="ETH">Ethereum (ETH)</option>
              <option value="USDC">USD Coin (USDC)</option>
              <option value="SOL">Solana (SOL)</option>
              <option value="ADA">Cardano (ADA)</option>
            </select>
          </div>

          <!-- Cantidad -->
          <div>
            <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Cantidad</label>
            <input v-model.number="cantidad" type="number" placeholder="0.0001" step="any" min="0"
              style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
          </div>

          <!-- Fecha y hora -->
          <div>
            <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Fecha y hora</label>
            <input v-model="fecha" type="datetime-local"
              style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
          </div>

          <!-- Precio ARS opcional -->
          <div>
            <label style="font-size: 13px; color: #aaa; display: block; margin-bottom: 4px;">Precio ARS <span style="color: #555;">(opcional)</span></label>
            <input v-model.number="precioArs" type="number" placeholder="auto"
              style="width: 100%; padding: 10px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
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
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Total ARS</th>
            <th style="text-align: left; padding: 10px; color: #aaa; font-size: 13px;">Fecha</th>
            <th style="padding: 10px;"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in transacciones" :key="t.id" style="border-bottom: 1px solid #222;">
            <td style="padding: 10px; font-weight: bold;">{{ t.simboloCripto }}</td>
            <td style="padding: 10px;" :style="{ color: t.tipo === 'Compra' ? '#51cf66' : '#ff6b6b' }">{{ t.tipo }}</td>
            <td style="padding: 10px;">{{ t.cantidad }}</td>
            <td style="padding: 10px;">${{ t.total }}</td>
            <td style="padding: 10px; font-size: 12px; color: #aaa;">{{ formatearFecha(t.fecha) }}</td>
            <td style="padding: 10px;">
              <div style="display: flex; gap: 6px;">
                <button @click="$router.push(`/transacciones/${t.id}`)"
                  style="padding: 4px 10px; border-radius: 5px; border: 1px solid #4dabf7; background: none; color: #4dabf7; cursor: pointer; font-size: 12px;">
                  Ver
                </button>
                <button @click="$router.push(`/transacciones/${t.id}/editar`)"
                  style="padding: 4px 10px; border-radius: 5px; border: 1px solid #ffd43b; background: none; color: #ffd43b; cursor: pointer; font-size: 12px;">
                  Editar
                </button>
                <button @click="confirmarBorrado(t.id)"
                  style="padding: 4px 10px; border-radius: 5px; border: 1px solid #ff6b6b; background: none; color: #ff6b6b; cursor: pointer; font-size: 12px;">
                  Borrar
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <p v-else style="color: #aaa;">Todavía no hay transacciones.</p>

      <!-- Modal confirmación borrado -->
      <div v-if="modalBorrar"
        style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); display: flex; align-items: center; justify-content: center; z-index: 100;">
        <div style="background: #1a1a2e; padding: 24px; border-radius: 10px; max-width: 350px; width: 90%; text-align: center;">
          <h3 style="margin-bottom: 12px;">¿Eliminar esta transacción?</h3>
          <p style="color: #aaa; font-size: 14px; margin-bottom: 20px;">Esta acción no se puede deshacer.</p>
          <div style="display: flex; gap: 10px; justify-content: center;">
            <button @click="modalBorrar = false"
              style="padding: 10px 20px; border-radius: 6px; border: 1px solid #333; background: none; color: #eee; cursor: pointer;">
              Cancelar
            </button>
            <button @click="eliminar"
              style="padding: 10px 20px; border-radius: 6px; border: none; background: #ff6b6b; color: #111; font-weight: bold; cursor: pointer;">
              Eliminar
            </button>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import Navbar from '../components/Navbar.vue'

const usuario = JSON.parse(localStorage.getItem('usuario') || '{}')

const tipo      = ref('Compra')
const simbolo   = ref('')
const cantidad  = ref(null)
const precioArs = ref(null)
const fecha     = ref(new Date().toISOString().slice(0, 16))
const error     = ref('')
const mensaje   = ref('')

const transacciones = ref([])
const cargando      = ref(true)
const modalBorrar   = ref(false)
const idABorrar     = ref(null)

async function cargarHistorial() {
  try {
    const respuesta = await axios.get(`http://localhost:5080/api/transacciones/usuario/${usuario.id}`)
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

  if (!simbolo.value)                   { error.value = 'Seleccioná una criptomoneda.'; return }
  if (!cantidad.value || cantidad.value <= 0) { error.value = 'La cantidad debe ser mayor a 0.'; return }
  if (!fecha.value)                     { error.value = 'Ingresá la fecha y hora.'; return }

  try {
    await axios.post('http://localhost:5080/api/transacciones', {
      usuarioId:     usuario.id,
      simboloCripto: simbolo.value,
      tipo:          tipo.value,
      cantidad:      cantidad.value,
      precioArs:     precioArs.value || 0,
      fecha:         fecha.value
    })

    mensaje.value   = `${tipo.value} registrada correctamente.`
    simbolo.value   = ''
    cantidad.value  = null
    precioArs.value = null
    fecha.value     = new Date().toISOString().slice(0, 16)

    await cargarHistorial()
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al registrar.'
  }
}

function confirmarBorrado(id) {
  idABorrar.value   = id
  modalBorrar.value = true
}

async function eliminar() {
  try {
    await axios.delete(`http://localhost:5080/api/transacciones/${idABorrar.value}`)
    modalBorrar.value = false
    await cargarHistorial()
  } catch {
    modalBorrar.value = false
  }
}

function formatearFecha(f) {
  return new Date(f).toLocaleString('es-AR', { dateStyle: 'short', timeStyle: 'short' })
}

onMounted(cargarHistorial)
</script>