<template>
  <div style="max-width: 350px; margin: 80px auto; padding: 24px; background: #1a1a2e; border-radius: 10px;">
    <h2 style="text-align: center; margin-bottom: 20px;">Iniciar sesión</h2>

    <p v-if="error" style="color: salmon; font-size: 14px;">{{ error }}</p>

    <div style="margin-bottom: 12px;">
      <label style="font-size: 13px; color: #aaa;">Usuario</label>
      <input v-model="nombreUsuario" placeholder="tu_usuario"
        style="width: 100%; padding: 10px; margin-top: 4px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;" />
    </div>

    <div style="margin-bottom: 16px;">
      <label style="font-size: 13px; color: #aaa;">Contraseña</label>
      <input v-model="contrasenia" type="password" placeholder="••••••"
        style="width: 100%; padding: 10px; margin-top: 4px; border-radius: 6px; border: 1px solid #333; background: #111; color: #eee;"
        @keyup.enter="iniciarSesion" />
    </div>

    <button @click="iniciarSesion"
      style="width: 100%; padding: 10px; background: #4dabf7; border: none; border-radius: 6px; color: #111; font-weight: bold; cursor: pointer;">
      Entrar
    </button>

    <p style="text-align: center; margin-top: 16px; font-size: 14px;">
      ¿No tenés cuenta? <RouterLink to="/registro" style="color: #4dabf7;">Registrate</RouterLink>
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const nombreUsuario = ref('')
const contrasenia = ref('')
const error = ref('')

async function iniciarSesion() {
  error.value = ''
  if (!nombreUsuario.value || !contrasenia.value) {
    error.value = 'Completá todos los campos.'
    return
  }
  try {
    const respuesta = await axios.post('http://localhost:5080/api/usuarios/login', {
      nombreUsuario: nombreUsuario.value,
      contrasenia: contrasenia.value
    })
    localStorage.setItem('usuario', JSON.stringify(respuesta.data))
    router.push('/dashboard')
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al iniciar sesión.'
  }
}
</script>