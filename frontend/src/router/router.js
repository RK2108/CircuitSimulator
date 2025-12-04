import { createWebHistory, createRouter } from "vue-router";
import CircuitBuilder from "@/screens/CircuitBuilder.vue";
import CircuitSelector from "@/screens/CircuitSelector.vue";

const routes = [
    {path: "/builder", component: CircuitBuilder},
    {path: "/selector", component: CircuitSelector}
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})