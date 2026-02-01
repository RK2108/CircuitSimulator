import { createApp } from 'vue'
import { router } from './router/router'
import App from './App.vue'

import 'vuetify/styles';
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const vuetify = createVuetify({
    components,
    directives,
    theme: {
        defaultTheme: 'light',
        themes: {
            light: {
                colors: {
                    primary: '#3b82f6',
                    secondary: '#6366f1'
                },
            },
        },
    },
})

createApp(App).use(router).use(vuetify).mount('#app')
