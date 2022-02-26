<template>
  <div class="w-screen h-screen">
    <Particles id="tsparticles" :options="particles" class="fixed" style="z-index: -10" />

    <div class="flex flex-col items-center w-full h-full justify-center">
      <q-card class="flex flex-col items-center rounded-3xl">
        <div class="mt-10 flex flex-row items-center">
          <!-- 这里替换成自己的icon -->
          <q-spinner-box color="orange" size="5.5em" />
          <span class="text-4xl font-extrabold">Heavens</span>
        </div>
        <span class="text-gray-400">Heavens 是基于 Quasar 设计的快速开发框架</span>
        <router-view />
      </q-card>

      <div class="absolute bottom-4">
        <span>版本号：0.1.0.0</span>
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { particles } from '.'
</script>
<style lang="sass" scoped></style>
