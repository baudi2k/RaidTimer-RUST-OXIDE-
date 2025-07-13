# RaidTimer Plugin para Rust

**Versión**: 3.0
**Autor**: ChatGPT para usuario Rust
**Compatibilidad**: Oxide/uMod

## 🚀 Descripción

**RaidTimer** es un plugin avanzado para servidores Rust que permite organizar eventos de raideo de forma estructurada y segura. Ofrece:

* Selección de jugadores mediante GUI
* Zona de raideo personalizada
* Temporizador configurable
* HUD con cuenta regresiva
* Protección de zona tras el evento

Ideal para servidores PvP organizados, torneos o eventos especiales.

---

## ⚖️ Características

* `/raidstart` abre un menú para elegir jugadores participantes.
* `/raidmark` permite marcar el centro de la zona de raideo.
* GUI para ingresar radio de protección (5m a 100m).
* GUI para definir tiempo de raideo en segundos.
* Temporizador visible en pantalla para jugadores seleccionados.
* Prevención de daño en la zona cuando finaliza el raideo.
* Comando `/raidstop` para cancelar el evento (requiere permiso).

---

## ✅ Permisos

| Permiso           | Descripción                                |
| ----------------- | ------------------------------------------ |
| `raidtimer.admin` | Permite cancelar el raideo con `/raidstop` |

Asignación de permisos:

```bash
oxide.grant user <steamID> raidtimer.admin
```

---

## ⚙️ Comandos

### Chat

```text
/raidstart   # Inicia la selección de jugadores
/raidmark    # Marca la zona de raideo
/raidstop    # Detiene el raideo (admin)
```

### Consola interna (no necesarias para jugadores)

* `raid.addplayer <id>` – Agrega jugador a la lista
* `raid.asktime` – Abre GUI de tiempo
* `raid.setradius <float>` – Establece radio de protección
* `raid.startinput <segundos>` – Inicia raideo con tiempo
* `raid.close <panel>` – Cierra UI especificada

---

## 🛠️ Instalación

1. Coloca el archivo `RaidTimer.cs` en la carpeta `oxide/plugins` de tu servidor Rust.
2. Reinicia el servidor o usa el comando:

```bash
oxide.reload RaidTimer
```

---

## 🚧 Recomendaciones

* Usar este plugin para eventos organizados.
* No iniciar un raideo sin marcar la zona.
* Limitar a máximo 8 jugadores por evento para evitar sobrecarga en GUI.

---

## 🌐 Crédito

Desarrollado por ChatGPT a solicitud de un usuario de servidores Rust. Adaptado para facilitar eventos PvP con mecánicas justas y controladas.

---

## 📁 Archivos

* `RaidTimer.cs`: Código fuente del plugin

---

## 🔧 Licencia

Este proyecto se distribuye con fines educativos y puede ser modificado libremente para uso en servidores Rust.
