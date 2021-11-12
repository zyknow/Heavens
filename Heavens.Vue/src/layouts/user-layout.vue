<template>
  <div class="w-screen h-screen bg-gray-100">
    <div class="w-full h-full flex-col flex-a-center">
      <div class="flex-all-center mt-40">
        <!-- 这里替换成自己的icon -->
        <q-spinner-box color="orange" size="5.5em" />
        <span class="text-4xl font-extrabold">Heavens</span>
      </div>
      <span class="text-gray-400">Heavens 基于Quasar设计的基础框架</span>
      <router-view></router-view>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { ref, defineComponent } from 'vue'
</script>
<style lang="sass"></style>
