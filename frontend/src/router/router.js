import { createWebHistory, createRouter } from "vue-router";
import CircuitBuilder from "@/screens/CircuitBuilder.vue";
import CircuitSelector from "@/screens/CircuitSelector.vue";

const routes = [
    {path: "/", component: CircuitSelector},
    {path: "/builder", component: CircuitBuilder}
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})