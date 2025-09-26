// Selectores
const formImc = document.querySelector("#form_imc");
const buttonResetFormImc = document.querySelector("#reset_form_imc");
const resultsBox = document.querySelector("#result_imc")
// Eventos

document.addEventListener("DOMContentLoaded", () => {
    addEventListeners()
})

function addEventListeners() {
    formImc.addEventListener("submit", evt => {
        evt.preventDefault()
        let { higth_imc: { value: valueHigth }, weigth_imc: { value: valueWeigth } } = evt.target

        valueHigth = Number(valueHigth)
        valueWeigth = Number(valueWeigth)

        caculateImc({ valueHigth, valueWeigth })
    })
    buttonResetFormImc.addEventListener("click", evt => {
        evt.preventDefault()
        resetForm()
    })
}

// Funciones de lógica
function caculateImc({ valueHigth, valueWeigth }) {
    valueHigth = valueHigth / 100
    const imc = (valueWeigth / (valueHigth * valueHigth)).toFixed(2)
    setValueResults(imc)
}


function resetForm() {
    // Reestablecer valores de formulario
    formImc.reset()
    // Reestablecer valor en resultado
    setValueResults("0")
}

// Comportamiento HTML

function setValueResults(value) {
    resultsBox.textContent = value
}