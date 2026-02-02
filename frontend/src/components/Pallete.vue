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
</style>