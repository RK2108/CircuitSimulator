<template>
    <v-navigation-drawer permanant width="200">
        <v-list density="compact">
            <v-list-subheader>TOOLS</v-list-subheader>

            <v-radio-group v-model="selected">
                <v-list-item>
                    <v-radio label="Battery" value="Battery">
                        <template v-slot:prepend>
                            <v-icon>mdi-battery</v-icon>
                        </template>
                    </v-radio>
                </v-list-item>

               <v-list-item>
                    <v-radio label="Resistor" value="Resistor">
                        <template v-slot:prepend>
                            <v-icon>mdi-resistor</v-icon>
                        </template>
                    </v-radio>
                </v-list-item>

               <v-list-item>
                    <v-radio label="Lamp" value="Lamp">
                        <template v-slot:prepend>
                            <v-icon>mdi-lightbulb</v-icon>
                        </template>
                    </v-radio>
                </v-list-item>

                <v-divider></v-divider>

                <v-list-item>
                    <v-radio label="Delete" value="Delete" color="error">
                        <template v-slot:prepend>
                            <v-icon>mdi-delete</v-icon>
                        </template>
                    </v-radio>
                </v-list-item>
            </v-radio-group>
        </v-list>
    </v-navigation-drawer>
</template>

<script setup>
    import { ref, watch } from 'vue';

    const selected = ref(null);

    watch(selected, (newVal) => {
        if (newVal){
            emit('select', newVal);
        }
    })

    const emit = defineEmits(['select']);
</script>

<style scoped>
    .pallete {
    width: 180px;
    background: #e2e8ee;
    border-right: 1px solid #e5e7eb;
    padding: 1rem;
    box-shadow: -2px 0 4px rgba(0, 0, 0, 0.03);
    }

    .pallete h3 {
    font-weight: 600;
    color: #374151;
    margin-bottom: 1rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    font-size: 0.9rem;
    }

    .options label {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 10px;
    margin-bottom: 6px;
    cursor: pointer;
    border: 1px solid #d1d5db;
    border-radius: 6px;
    transition: all 0.2s ease;
    background: #ffffff;
    font-size: 0.9rem;
    }

    .options input[type='radio'] {
    accent-color: #3b82f6;
    }

    .options label:hover {
    background: #eef2ff;
    border-color: #6366f1;
    }
</style>