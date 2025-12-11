# 📊 Gestor de Gastos Personales - Documentación Completa

## 🎯 Descripción General del Proyecto

Este es un **sistema web de gestión de gastos personales** desarrollado con **ASP.NET Core MVC** y **PostgreSQL**, que permite a los usuarios registrar sus gastos, establecer presupuestos y visualizar estadísticas detalladas de sus finanzas.

---

## 🏗️ Arquitectura del Proyecto

### **Patrón de Diseño: MVC (Model-View-Controller)**

El proyecto sigue el patrón **MVC** de forma estricta:

```
final_project/
│
├── Models/              # 🗂️ Modelos de datos (Entidades)
├── Views/               # 🎨 Interfaces de usuario (HTML/Razor)
├── Controllers/         # 🎮 Lógica de control y flujo
├── Data/                # 💾 Contexto de base de datos
├── ViewModels/          # 📦 Modelos específicos para vistas
└── wwwroot/             # 🌐 Recursos estáticos (CSS, JS, imágenes)
```

---

## 📦 Componentes Principales

### **1. Modelos (Models/)**

Los modelos representan las **entidades de negocio** y definen la estructura de la base de datos.

#### **Gasto.cs**
Representa un gasto individual del usuario.

**Propiedades principales:**
- `Id`: Identificador único
- `Descripcion`: Descripción del gasto (ej: "Almuerzo")
- `Monto`: Cantidad gastada (decimal)
- `Categoria`: Clasificación (Alimentación, Transporte, etc.)
- `Fecha`: Fecha del gasto
- `Notas`: Información adicional opcional

**Validaciones implementadas:**
```csharp
[Required(ErrorMessage = "La descripción es obligatoria")]
[StringLength(200, MinimumLength = 3)]
[Range(0.01, 999999999.99)]
```

#### **Presupuesto.cs**
Define el presupuesto para un periodo específico.

**Propiedades principales:**
- `Id`: Identificador único
- `MontoTotal`: Cantidad total del presupuesto
- `FechaInicio` / `FechaFin`: Periodo de validez
- `Descripcion`: Nota descriptiva
- `Activo`: Indica si es el presupuesto actual

**Propiedades calculadas:**
- `DuracionDias`: Duración del periodo
- `EstaVigente`: Si está en el periodo actual
- `HaFinalizado`: Si ya pasó la fecha fin

---

### **2. Controladores (Controllers/)**

Los controladores manejan la **lógica de negocio** y coordinan entre modelos y vistas.

#### **HomeController.cs**
- **Función**: Página principal (Dashboard)
- **Responsabilidades:**
  - Mostrar resumen de estadísticas
  - Formulario de gasto rápido
  - Últimos 5 gastos registrados
  - Top 3 categorías con más gastos

**Métodos principales:**
```csharp
Index()                    // Dashboard principal
AgregarGastoRapido()       // Crear gasto desde el dashboard
```

#### **GastosController.cs**
- **Función**: Gestión completa de gastos (CRUD)
- **Operaciones:**
  - `Index()`: Lista todos los gastos con estadísticas
  - `Create()`: Formulario y creación de nuevo gasto
  - `Edit()`: Modificación de gasto existente
  - `Delete()`: Eliminación con confirmación
  - `Details()`: Vista detallada de un gasto

**Características destacadas:**
- Validaciones del lado del servidor
- Manejo de errores con try-catch
- Logging de operaciones
- Mensajes flash con TempData

#### **PresupuestoController.cs**
- **Función**: Administración de presupuestos
- **Operaciones:**
  - `Index()`: Vista del presupuesto activo
  - `Create()`: Crear nuevo presupuesto
  - `Edit()`: Modificar presupuesto existente
  - `Delete()`: Eliminar presupuesto

**Lógica especial:**
- Solo un presupuesto puede estar activo a la vez
- Al activar uno, se desactivan los demás automáticamente
- Cálculos de gastos dentro del periodo del presupuesto

---

### **3. Vistas (Views/)**

Las vistas utilizan **Razor** para generar HTML dinámico.

#### **Estructura de carpetas:**
```
Views/
├── Home/
│   ├── Index.cshtml           # Dashboard principal
│   └── Privacy.cshtml          # Página de privacidad
│
├── Gastos/
│   ├── Index.cshtml            # Lista de gastos
│   ├── Create.cshtml           # Crear gasto
│   ├── Edit.cshtml             # Editar gasto
│   ├── Delete.cshtml           # Confirmar eliminación
│   └── Details.cshtml          # Detalles del gasto
│
├── Presupuesto/
│   ├── Index.cshtml            # Ver presupuesto
│   ├── Create.cshtml           # Crear presupuesto
│   └── Edit.cshtml             # Editar presupuesto
│
└── Shared/
    ├── _Layout.cshtml          # Plantilla principal
    └── _ValidationScriptsPartial.cshtml
```

#### **Características de las vistas:**
- **Bootstrap 5** para diseño responsive
- **Bootstrap Icons** para iconografía
- **Validaciones HTML5** nativas
- **Mensajes flash** con TempData
- **Animaciones CSS** para mejor UX

---

### **4. ViewModels (ViewModels/)**

Los ViewModels son **modelos específicos** para las vistas que combinan datos de múltiples fuentes.

#### **DashboardViewModel.cs**
Agrupa toda la información del dashboard:
```csharp
- PresupuestoActivo
- TotalGastado
- UltimosGastos
- TopCategorias
- PorcentajeUsado
```

#### **GastosIndexViewModel.cs**
Datos para la vista de lista de gastos:
```csharp
- Gastos (lista)
- TotalGastado
- GastosPorCategoria
- PresupuestoTotal
```

#### **PresupuestoIndexViewModel.cs**
Información del presupuesto actual:
```csharp
- PresupuestoActivo
- TotalGastado
- Restante
- DiasRestantes
- GastoProyectado
```

---

### **5. Acceso a Datos (Data/)**

#### **ApplicationDbContext.cs**
Contexto de Entity Framework Core que maneja la conexión con PostgreSQL.

**Configuración:**
```csharp
public DbSet<Gasto> Gastos { get; set; }
public DbSet<Presupuesto> Presupuestos { get; set; }
```

**Características:**
- Usa **Code First** approach
- Configuración de tipos de columnas para PostgreSQL
- `OnModelCreating()` para configuraciones avanzadas

---

## 🎨 Capa de Presentación

### **CSS Personalizado (wwwroot/css/custom.css)**

**Características implementadas:**
- Variables CSS para colores consistentes
- Animaciones de entrada (`fadeInUp`)
- Hover effects en tarjetas y botones
- Estilos para badges por categoría
- Diseño responsive con media queries
- Gradientes en tarjetas de estadísticas
- Transiciones suaves

### **JavaScript (wwwroot/js/validation.js)**

**Funcionalidades:**
- Validación en tiempo real de formularios
- Confirmaciones antes de eliminar
- Auto-cierre de alertas después de 5 segundos
- Formateo automático de montos
- Contador de caracteres en textareas
- Prevención de envíos múltiples
- Detección de cambios sin guardar

---

## 🔄 Flujo de Datos

### **Ejemplo: Crear un Gasto**

```
1. Usuario → Vista (Create.cshtml)
   ↓
2. Formulario → Validación JavaScript
   ↓
3. Submit → POST a GastosController.Create()
   ↓
4. Validación del Modelo (Data Annotations)
   ↓
5. Si es válido:
   - Guardar en DbContext
   - SaveChangesAsync()
   - TempData["Mensaje"] = "Éxito"
   - Redirect a Index
   ↓
6. Vista Index muestra el nuevo gasto
```

---

## 🗃️ Base de Datos

### **Estructura de Tablas**

#### **Tabla: Gastos**
```sql
Columnas:
- Id (int, PK)
- Descripcion (varchar(200))
- Monto (decimal(18,2))
- Categoria (varchar(50))
- Fecha (timestamp)
- Notas (varchar(500), nullable)
```

#### **Tabla: Presupuestos**
```sql
Columnas:
- Id (int, PK)
- MontoTotal (decimal(18,2))
- FechaInicio (timestamp)
- FechaFin (timestamp)
- Descripcion (varchar(200), nullable)
- Activo (boolean)
```

### **Relaciones**
Actualmente no hay relaciones FK entre tablas. Los gastos se filtran por fechas del presupuesto usando **LINQ**.

---

## 🔍 Consultas LINQ Principales

### **Obtener gastos del periodo activo:**
```csharp
var gastosDelPeriodo = await _context.Gastos
    .Where(g => g.Fecha >= presupuesto.FechaInicio && 
                g.Fecha <= presupuesto.FechaFin)
    .ToListAsync();
```

### **Gastos por categoría:**
```csharp
var gastosPorCategoria = gastos
    .GroupBy(g => g.Categoria)
    .Select(group => new {
        Categoria = group.Key,
        Total = group.Sum(g => g.Monto),
        Cantidad = group.Count()
    })
    .OrderByDescending(x => x.Total)
    .ToList();
```

### **Presupuesto activo:**
```csharp
var presupuestoActivo = await _context.Presupuestos
    .Where(p => p.Activo && 
                p.FechaInicio <= DateTime.Now && 
                p.FechaFin >= DateTime.Now)
    .FirstOrDefaultAsync();
```

---

## ⚙️ Configuración (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=final_project;Username=postgres;Password=***"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🛡️ Validaciones Implementadas

### **Del lado del servidor (C#):**
- Data Annotations en modelos
- Validaciones personalizadas en controladores
- ModelState.IsValid antes de guardar
- Try-catch para manejo de errores

### **Del lado del cliente (JavaScript):**
- HTML5 validation attributes
- Validación en tiempo real
- Confirmaciones antes de acciones destructivas
- Formateo automático de campos

---

## 🎯 Características Destacadas

### **1. Dashboard Intuitivo**
- Resumen visual de estadísticas
- Tarjetas con gradientes y animaciones
- Formulario de gasto rápido
- Top categorías más gastadas

### **2. Gestión Completa de Gastos**
- CRUD completo
- Búsqueda y filtrado
- Estadísticas por categoría
- Visualización con barras de progreso

### **3. Control de Presupuesto**
- Seguimiento en tiempo real
- Alertas de límite
- Cálculo automático de restante
- Historial de presupuestos

### **4. Experiencia de Usuario**
- Diseño responsive
- Animaciones suaves
- Mensajes informativos
- Validaciones claras
- Tooltips de ayuda

---

## 🚀 Tecnologías Utilizadas

| Tecnología | Versión | Uso |
|-----------|---------|-----|
| .NET | 9.0 | Framework principal |
| ASP.NET Core MVC | 9.0 | Patrón de arquitectura |
| Entity Framework Core | 9.0 | ORM para base de datos |
| PostgreSQL | Latest | Base de datos |
| Npgsql | Latest | Provider de PostgreSQL |
| Bootstrap | 5.x | Framework CSS |
| Bootstrap Icons | Latest | Iconografía |
| jQuery | 3.x | Manipulación DOM |
| JavaScript (ES6+) | - | Validaciones cliente |

---

## 📊 Flujo de Navegación

```
┌─────────────┐
│   Home      │ ← Dashboard principal con estadísticas
└──────┬──────┘
       │
       ├─────→ Gastos/Index ─────→ Gastos/Create
       │           │                     │
       │           ├─────→ Gastos/Edit  │
       │           │                     │
       │           └─────→ Gastos/Delete│
       │                                 │
       └─────→ Presupuesto/Index ───────┴───→ Presupuesto/Create
                   │
                   └─────→ Presupuesto/Edit
```

---

## 🔐 Seguridad Implementada

1. **ValidateAntiForgeryToken**: Protección CSRF en formularios
2. **Data Annotations**: Validación de entrada de datos
3. **Try-Catch**: Manejo de excepciones
4. **Logging**: Registro de operaciones importantes
5. **SQL Injection**: Prevención con Entity Framework (parameterización automática)

---

## 📈 Métricas y Estadísticas Calculadas

### **En el Dashboard:**
- Total gastado (suma de todos los gastos)
- Presupuesto restante
- Porcentaje usado del presupuesto
- Gasto del mes actual
- Top 3 categorías

### **En la vista de Gastos:**
- Total por categoría
- Porcentaje por categoría
- Cantidad de gastos por categoría
- Comparación con presupuesto

### **En la vista de Presupuesto:**
- Días restantes
- Días transcurridos
- Promedio de gasto diario
- Proyección de gasto al final del periodo

---

## 🎨 Paleta de Colores

```css
--primary-color: #0d6efd    (Azul - Información)
--success-color: #198754    (Verde - Éxito/Restante)
--danger-color: #dc3545     (Rojo - Gastos/Eliminar)
--warning-color: #ffc107    (Amarillo - Alertas/Editar)
--info-color: #0dcaf0       (Cyan - Información adicional)
```

---

## 🔄 Ciclo de Vida de una Petición

```
1. Usuario hace clic en "Crear Gasto"
   ↓
2. Navegador solicita GET /Gastos/Create
   ↓
3. GastosController.Create() (GET)
   ↓
4. Retorna vista Create.cshtml con modelo vacío
   ↓
5. Usuario llena formulario y hace submit
   ↓
6. POST /Gastos/Create con datos del formulario
   ↓
7. GastosController.Create(Gasto) (POST)
   ↓
8. Validación del modelo
   ↓
9. Si válido: Guardar en BD → Redirect a Index
   Si inválido: Retornar vista con errores
   ↓
10. Usuario ve lista actualizada con mensaje de éxito
```

---

## 💡 Buenas Prácticas Implementadas

1. **Separación de responsabilidades** (MVC)
2. **ViewModels** para vistas complejas
3. **Repository pattern** (a través de DbContext)
4. **Dependency Injection** (servicios en Program.cs)
5. **Async/Await** para operaciones de BD
6. **Logging** para debugging
7. **Try-Catch** para manejo de errores
8. **TempData** para mensajes flash
9. **Validaciones** en cliente y servidor
10. **Código limpio** y comentado

---

## 🎓 Conceptos Clave Aplicados

- **MVC Pattern**: Separación clara de capas
- **CRUD Operations**: Create, Read, Update, Delete
- **ORM**: Entity Framework Core
- **LINQ**: Consultas en memoria y BD
- **Razor Syntax**: Mezcla de C# y HTML
- **Tag Helpers**: Simplificación de formularios
- **Data Annotations**: Validaciones declarativas
- **Bootstrap**: Diseño responsive
- **JavaScript Events**: Interactividad cliente
- **Async Programming**: Operaciones no bloqueantes

---

## 📝 Resumen Ejecutivo

Este proyecto es una **aplicación web full-stack** que demuestra:

✅ Dominio de **ASP.NET Core MVC**  
✅ Integración con **PostgreSQL**  
✅ Uso de **Entity Framework Core**  
✅ Consultas con **LINQ**  
✅ **Validaciones** robustas  
✅ **UI/UX** moderna y responsive  
✅ **Manejo de errores** apropiado  
✅ **Buenas prácticas** de desarrollo  

Es ideal como **proyecto de portafolio** que demuestra habilidades en desarrollo web con .NET.

---

## 🔮 Posibles Mejoras Futuras

1. Autenticación de usuarios (Identity)
2. Gráficos con Chart.js
3. Exportación a Excel/PDF
4. API REST para móvil
5. Gastos recurrentes
6. Múltiples presupuestos por categoría
7. Notificaciones por email
8. Dark mode
9. PWA (Progressive Web App)
10. Predicción de gastos con ML

---

**Desarrollado con ❤️ usando ASP.NET Core 9.0 y PostgreSQL**
