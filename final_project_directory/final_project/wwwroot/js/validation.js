// ============================================
// GESTOR DE GASTOS - VALIDACIONES CLIENTE
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // Validación de fechas en formularios de presupuesto
    const formPresupuesto = document.querySelector('form[action*="Presupuesto"]');
    if (formPresupuesto) {
        formPresupuesto.addEventListener('submit', function (e) {
            const fechaInicio = document.getElementById('FechaInicio');
            const fechaFin = document.getElementById('FechaFin');

            if (fechaInicio && fechaFin) {
                const inicio = new Date(fechaInicio.value);
                const fin = new Date(fechaFin.value);

                if (fin <= inicio) {
                    e.preventDefault();
                    alert('⚠️ La fecha de fin debe ser posterior a la fecha de inicio');
                    fechaFin.focus();
                    return false;
                }
            }
        });
    }

    // Validación de gastos
    const formGasto = document.querySelector('form[action*="Gastos"], form[action*="AgregarGastoRapido"]');
    if (formGasto) {
        formGasto.addEventListener('submit', function (e) {
            const monto = document.getElementById('Monto');
            const fecha = document.getElementById('Fecha');

            // Validar monto positivo
            if (monto && parseFloat(monto.value) <= 0) {
                e.preventDefault();
                alert('⚠️ El monto debe ser mayor a 0');
                monto.focus();
                return false;
            }

            // Validar que la fecha no sea futura
            if (fecha) {
                const fechaGasto = new Date(fecha.value);
                const hoy = new Date();
                hoy.setHours(0, 0, 0, 0);

                if (fechaGasto > hoy) {
                    const confirmar = confirm('⚠️ La fecha seleccionada es futura. ¿Deseas continuar?');
                    if (!confirmar) {
                        e.preventDefault();
                        fecha.focus();
                        return false;
                    }
                }
            }
        });
    }

    // Auto-cerrar alertas después de 5 segundos
    const alertas = document.querySelectorAll('.alert:not(.alert-permanent)');
    alertas.forEach(function (alerta) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alerta);
            bsAlert.close();
        }, 5000);
    });

    // Confirmación antes de eliminar
    const botonesEliminar = document.querySelectorAll('a[href*="/Delete/"], button[formaction*="/Delete/"]');
    botonesEliminar.forEach(function (boton) {
        boton.addEventListener('click', function (e) {
            // Si no estamos en la página de confirmación
            if (!window.location.href.includes('/Delete/')) {
                e.preventDefault();
                const confirmar = confirm('🗑️ ¿Estás seguro de que deseas eliminar este registro? Esta acción no se puede deshacer.');
                if (confirmar) {
                    window.location.href = this.href;
                }
            }
        });
    });

    // Formatear montos mientras se escribe
    const inputsMontos = document.querySelectorAll('input[type="number"][step="0.01"]');
    inputsMontos.forEach(function (input) {
        input.addEventListener('blur', function () {
            if (this.value) {
                const valor = parseFloat(this.value);
                if (!isNaN(valor)) {
                    this.value = valor.toFixed(2);
                }
            }
        });
    });

    // Validación en tiempo real para campos requeridos
    const camposRequeridos = document.querySelectorAll('input[required], select[required], textarea[required]');
    camposRequeridos.forEach(function (campo) {
        campo.addEventListener('blur', function () {
            if (!this.value.trim()) {
                this.classList.add('is-invalid');
            } else {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            }
        });

        campo.addEventListener('input', function () {
            if (this.value.trim()) {
                this.classList.remove('is-invalid');
            }
        });
    });

    // Prevenir envío múltiple de formularios
    const formularios = document.querySelectorAll('form');
    formularios.forEach(function (form) {
        form.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando...';

                // Re-habilitar después de 3 segundos por si hay error
                setTimeout(function () {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = submitBtn.getAttribute('data-original-text') || 'Guardar';
                }, 3000);
            }
        });
    });

    // Guardar texto original de botones para restaurar
    document.querySelectorAll('button[type="submit"]').forEach(function (btn) {
        btn.setAttribute('data-original-text', btn.innerHTML);
    });

    // Contador de caracteres para textareas
    const textareas = document.querySelectorAll('textarea[maxlength]');
    textareas.forEach(function (textarea) {
        const maxLength = textarea.getAttribute('maxlength');
        const contador = document.createElement('small');
        contador.className = 'form-text text-muted';
        contador.style.float = 'right';
        textarea.parentNode.appendChild(contador);

        const actualizarContador = function () {
            const restante = maxLength - textarea.value.length;
            contador.textContent = `${restante} caracteres restantes`;
            contador.style.color = restante < 50 ? '#dc3545' : '#6c757d';
        };

        textarea.addEventListener('input', actualizarContador);
        actualizarContador();
    });

    // Tooltip para iconos de ayuda
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(function (tooltip) {
        new bootstrap.Tooltip(tooltip);
    });

    console.log('✅ Validaciones del lado del cliente cargadas correctamente');
});

// Función auxiliar para formatear moneda
function formatCurrency(amount) {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP'
    }).format(amount);
}

// Función para confirmar acciones destructivas
function confirmarAccion(mensaje) {
    return confirm(mensaje || '¿Estás seguro de realizar esta acción?');
}