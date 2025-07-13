# RaidTimer Plugin para Rust

**Versión**: 3.1
**Autor**: ChatGPT para usuario Rust
**Compatibilidad**: Oxide/uMod

## 🚀 Descripción

**RaidTimer** es un plugin para Rust que organiza eventos de raideo con detección automática de bases, zona protegida y una GUI intuitiva. La versión 3.1 introduce mejoras clave para automatizar y simplificar el proceso.

---

## ⚖️ Características

* Detección automática de la base mediante el Tool Cupboard más cercano.
* Cálculo automático del centro y radio de la zona de raideo.
* Selección de jugadores mediante GUI simplificada.
* Ingreso de tiempo de raideo mediante GUI.
* HUD visible con temporizador para jugadores seleccionados.
* Zona protegida al finalizar el raideo (sin daños).
* Cancelación de raideo por administradores.

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
/raidmark    # Detecta automáticamente la base (TC)
/raidstop    # Detiene el raideo (admin)
```

### Consola interna

* `raid.addplayer <id>`
* `raid.asktime`
* `raid.startinput <segundos>`
* `raid.close <panel>`

---

## 🛠️ Instalación

1. Coloca `RaidTimer.cs` en la carpeta `oxide/plugins`.
2. Usa el comando:

```bash
oxide.reload RaidTimer
```

---

## 🚧 Notas

* El sistema detecta automáticamente la base con bloques asociados al Tool Cupboard.
* La zona se marca con el centro y radio exactos de la construcción.
* Si no se encuentra un TC válido, no se podrá iniciar el raideo.

---

## 🌐 Crédito

Desarrollado por ChatGPT según las necesidades de usuarios de servidores Rust. Esta versión automatiza el proceso de selección de zonas para facilitar eventos justos y rápidos.

---

## 📁 Archivos

* `RaidTimer.cs`: Plugin principal

---

## 🔧 Licencia

Uso libre con fines educativos o comunitarios en servidores Rust.
