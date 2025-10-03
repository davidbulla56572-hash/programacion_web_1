function setValueBoxHtml(element, value = "") {
    element.textContent = value
}

// ==============================
// Ejercicio 1: Conversor
// ==============================
const tempFieldHtml = document.querySelector('#temp')
const typeFieldHtml = document.querySelector('#tipo')
const errorConversionBoxHtml = document.querySelector('#error1')
const resultConversionBoxHtml = document.querySelector('#resultado1')

const labelsByConversionType = {
    "c": "Celsius",
    "f": "Fahrenheit"
}

document.getElementById("btnConvertir").addEventListener("click", function () {
    // TODO: Implementar validación y conversión
    setValueBoxHtml(errorConversionBoxHtml)
    setValueBoxHtml(resultConversionBoxHtml)

    // Tomar el valor y el tipo
    const valueTemp = (tempFieldHtml.value).trim()
    const typeConversion = typeFieldHtml.value

    // Validar si el valor ingresado es válido
    if (isNaN(valueTemp) || valueTemp == "") {
        const errorMessage = "No es posible la conversión, establece un valor numérico"
        setValueBoxHtml(errorConversionBoxHtml, errorMessage)
        return
    }
    // Calcular según el tipo de conversión
    const conversions = {
        "c": value => {
            return ((value - 32) * (5 / 9)).toFixed(2)
        },
        "f": value => {
            return ((value * (9 / 5)) + 32).toFixed(2)
        }
    }

    // mostrar el resultado
    const valueCalculated = conversions[typeConversion](Number(valueTemp))
    const message = `La conversión a [${labelsByConversionType[typeConversion]}] es: ${valueCalculated}`
    setValueBoxHtml(resultConversionBoxHtml, message)

});

// ==============================
// Ejercicio 2: Calculadora TMB
// ==============================
const nameHtml = document.querySelector("#nombre")
const yearsOldHtml = document.querySelector("#edad")
const weightHtml = document.querySelector("#peso")
const heightHtml = document.querySelector("#altura")
const sexHtml = document.querySelector("#sexo")
const resultBoxTmb = document.querySelector("#resultado2")
const errorBoxTmb = document.querySelector("#error2")

document.getElementById("btnCalcular").addEventListener("click", function () {
    // TODO: Validar campos y calcular TMB
    // Limpia el HTML
    setValueBoxHtml(resultBoxTmb)
    setValueBoxHtml(errorBoxTmb)

    // Tomar los valores
    const height = (heightHtml.value).trim()
    const weight = (weightHtml.value).trim()
    const yearsOld = (yearsOldHtml.value).trim()
    const name = (nameHtml.value).trim()
    const sex = sexHtml.value
    // Validar los valores
    if (
        sex === "" ||
        name === "" ||
        height === "" ||
        weight === "" ||
        yearsOld === ""
    ) {
        const messageError = "Debes de completar todos los campos de forma correcta"
        setValueBoxHtml(errorBoxTmb, messageError)
        return
    }

    if (
        isNaN(height) ||
        isNaN(weight) ||
        isNaN(yearsOld)
    ) {
        const messageError = "Los campos deben de ser numéricos"
        return setValueBoxHtml(errorBoxTmb, messageError)
    }

    // Elegir tipo de cálculo según el sexo
    const calculateTmb = {
        "hombre": (height, weight, yearsOld) => {
            return (10 * weight) + (6.25 * height) - (5 * yearsOld) + 5
        },
        "mujer": (height, weight, yearsOld) => {
            return (10 * weight) + (6.25 * height) - (5 * yearsOld) - 161
        }
    }
    // Mostrar reporte final de lo calculado
    const valueCalculated = (calculateTmb[sex])(height, weight, yearsOld).toFixed(2)
    const message = `Calorías necesarias en estado reposo: ${valueCalculated}`
    setValueBoxHtml(resultBoxTmb, message)
});