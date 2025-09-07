const boton = document.querySelector("#boton_1")
const titleBox = document.querySelector("#title")

boton.addEventListener("click", () => {
    titleBox.textContent = "Titulo cambiado"
})
