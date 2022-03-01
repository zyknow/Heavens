<template>
  <div>
    <h1>Home</h1>
    <transition appear enter-active-class="animated jackInTheBox" leave-active-class="animated fadeOut">
      <!-- Wrapping only one DOM element, defined by QBtn -->
      <q-btn v-show="show" color="secondary" icon="mail" label="Email" />
    </transition>
    <q-btn color="secondary" icon="mail" label="Email" @click="test" />
    <q-tabs
      align="left"
      active-color="primary"
      dense
      inline-label
      indicator-color="transparent"
      :breakpoint="0"
      outside-arrows
      class="text-teal"
    >
      <q-tab :ripple="false" name="mails" icon="mail" label="Mails" />
      <div class="flex flex-row flex-nowrap items-center justify-center">
        <q-tab name="movies3" class="pointer-events-none w-0" />
        <span>Movis</span>
      </div>
      <q-tab name="alarms" icon="alarm" label="Alarms" />
      <q-tab name="movies" icon="movie" label="Movies" />
      <q-tab name="movies1" icon="movie" label="Movies" />
      <q-tab name="movies2" icon="movie" label="Movies" />
    </q-tabs>
  </div>
</template>
<script lang="ts">
import { ref } from 'vue'

export default {
  name: 'Home'
}
</script>

<script lang="ts" setup>
const show = ref(false)

const test = () => {
  show.value = !show.value
}
</script>
<style lang="sass"></style>
