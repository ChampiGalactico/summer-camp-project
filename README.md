# [Nombre del juego — pendiente de definir]

> Juego cooperativo de puzzles y exploración para 2 jugadores (LAN), desarrollado para el Summer Camp "Videojuegos + IA".


## La idea en una frase

Dos personas firman para ser sujetos de prueba en un experimento psicológico dirigido por una IA, con la finalidad declarada de analizar la naturaleza del egoísmo y la confianza humana — para desarrollar técnicas que debiliten la confianza y la cohesión entre personas (útil en contexto militar-académico para interrogatorios, contrainteligencia, control de grupos). El experimento resulta ser una simulación mental compartida: deben cooperar si quieren terminarlo con "éxito". Desde el inicio de la simulación conocen las reglas: si ambos sobreviven, cada uno cobra su pago; si solo uno sobrevive, se lleva el pago de los dos; y si alguno muere ahí dentro, muere también en la vida real.

Uno de los jugadores es el **Guía** (ve el mapa, no ve el entorno real, investiga y craftea desde su cuarto). El otro es el **Corredor** (explora el entorno real con visión limitada, se comunica solo por altavoz físico — su voz real genera ruido real que las criaturas detectan). Juntos deben resolver puzzles, sobrevivir, y decidir hasta dónde llega su cooperación cuando traicionar es más rentable.

**La comandante IA** de la instalación mide, con humor ácido y total indiferencia, qué tan fácil es romper la confianza humana — y no solo observa el egoísmo, lo provoca.

## Documentos del proyecto

- **`Propuesta_Juego_Summer_Camp.md`** — Documento conceptual: premisa narrativa, roles, pilares, sistema de vida/voluntad, estructura de dos actos, finales, la comandante IA, y cómo presentar la IA para la temática del evento.
- **`Propuesta_Tecnica_Juego_Summer_Camp.md`** — Documento técnico: tipos de sala, biomas, criaturas, capacidades específicas de cada rol, diseño detallado de la apertura y el Laberinto 1 (incluyendo la Sala del Guía 1), puzzles y reglas de densidad.
- **`/referencias`** *(sugerido)* — Imágenes y juegos de referencia (estética de biomas, mecánicas similares, mood boards).

## Referencias de diseño (juegos y películas de referencia)

- **Escape Academy** — comunicación cooperativa bajo presión, resolución de puzzles en equipo con información asimétrica.
- **It Takes Two** — mecánicas asimétricas entre dos jugadores, cada rol con herramientas y perspectiva distintas.
- **Maze Runner** (película) — estética y premisa del laberinto, "El Claro" como punto de partida seguro, la sensación de instalación que oculta más de lo que muestra.
- **Dead by Daylight** — tensión superviviente-cazador, gestión de riesgo frente a una amenaza que patrulla el entorno.
- **The Forest** — supervivencia y crafteo en un entorno hostil, sensación de escasez de recursos como motor de tensión.

---

## Pendientes — Narrativa

- [ ] **Nombre del juego** (título de trabajo actual: ninguno definido).
- [ ] Contenido exacto de los 3 easter eggs (uno por bioma en el Laberinto 1) — el de Control tiene una idea tentativa (posible referencia al psicoanálisis), sin confirmar. Aislamiento y Fricción: sin definir.
- [ ] Guion completo de la comandante IA: el discurso inicial (propósito, reglas de pago, y la muerte real, todo anunciado desde el inicio de la simulación), líneas de duda sembrada, líneas de culpa repartida, y el monólogo de la escena del falso final.
- [ ] Contenido de las "historias random" del Cuaderno de campo (Sala del Guía 1) — actualmente solo está definida su función, no su contenido.
- [ ] Reportes de laboratorio de ambas criaturas — solo está redactado el ejemplo de referencia de la Criatura 1; falta el de la Criatura 2 y pulir el tono final de ambos.
- [ ] Diálogo específico de la recepcionista en la Escena 0 (solo hay una línea de referencia, falta el guion completo).
- [ ] Estructura narrativa del Laberinto 2 con el mismo nivel de detalle que el Laberinto 1 (biomas Óxido, Codicia, Vacío ya tienen concepto, falta contenido sala por sala).
- [ ] Diseño narrativo del tramo de la salida real del Laberinto 2 (dónde encuentra el Corredor la llave/tarjeta) y del regreso conjunto por el compañero.

## Pendientes — Mecánicas

- [ ] Balance numérico completo: velocidad de decaimiento de vida del Guía, costo de voluntad por tipo de acción, velocidad de regeneración de voluntad, tamaño de la barra y número de vidas del Corredor.
- [ ] Radio y velocidad de detección de ambas criaturas (sonido y luz), y patrones de patrullaje.
- [ ] Decidir si la linterna del Corredor tiene límite de batería/uso propio, o es de uso libre.
- [ ] Cantidad exacta de batería de la cámara crafteable en cada laberinto, y cuántos materiales de criatura derrotada se necesitan para craftear una batería adicional.
- [ ] Layout sala por sala completo del Laberinto 2 (solo está definido el concepto de bioma y la mecánica característica del monstruo de luz).
- [ ] Contenido específico de los puzzles restantes de Aislamiento y Fricción (aplicando la regla de 2-3 puzzles + 1 easter egg por bioma, ya definida para Control).
- [ ] Diseño concreto del tramo de regreso conjunto (Guía + Corredor) tras abrir la puerta en el clímax del Acto 2, y qué puede hacer que mueran los dos ahí.
- [ ] Duración estimada por sala, por bioma y por laberinto completo.
- [ ] Arte de referencia final por bioma (paleta de color exacta, assets de Unreal Marketplace vs. hechos a mano).

## Preguntas y vacíos abiertos

- ¿Qué pasa exactamente si el Corredor cae en el Camino 1 ("muerte inminente") pero era su última vida — hay alguna diferencia narrativa entre perder ahí vs. perder en cualquier otro punto del laberinto?
- ¿La información de "última posición conocida" de una criatura (cuando sale del radio de percepción del Guía) se muestra de alguna forma en el mapa, o simplemente desaparece?
- ¿Existen más de 2 tipos de criatura a futuro, o el juego se mantiene con solo Criatura 1 y Criatura 2 para esta versión del summer camp?
- ¿El "Receptor" del Guía y la maleta del Corredor tienen alguna representación visual/diegética further (por ejemplo, un diseño concreto de UI), o eso se deja para la fase de arte?
- Escalar a 3+ jugadores y/o modo online: sigue como visión a futuro — ¿en algún punto de este summer camp se quiere dejar la arquitectura ya preparada para eso, o se aborda después de la entrega?

## Metodología

Para el desarrollo de esta propuesta se utilizó a Claude (Anthropic) como herramienta de apoyo iterativo durante todo el proceso de diseño. El flujo de trabajo consistió en plantear ideas y mecánicas de forma conversacional, mientras Claude hacía preguntas orientadoras para resolver ambigüedades de diseño antes de avanzar (por ejemplo, aclarar si un sistema era simétrico entre roles, o qué pasaba exactamente en un caso límite de una mecánica). Periódicamente se le pidió a Claude que analizara el conjunto de decisiones tomadas hasta ese punto para identificar vacíos narrativos y mecánicos —piezas del diseño que se daban por sentadas pero no estaban realmente resueltas, como la falta de consecuencia real en el cambio de roles, o la ausencia de una razón física para separar a los jugadores durante la sesión de juego—, lo cual permitió detectar y corregir inconsistencias de forma temprana en vez de durante el desarrollo. Claude también ayudó a estructurar y documentar las decisiones ya tomadas en dos documentos de trabajo (uno conceptual y uno técnico), manteniéndolos actualizados a medida que el diseño evolucionaba. En ningún momento se usó Claude para generar la dirección creativa del proyecto de forma autónoma: cada mecánica, giro narrativo y decisión de diseño partió de propuestas propias del equipo, con Claude actuando como herramienta de refinamiento, detección de vacíos y documentación.


*Este README y demás archivos son netamente tentativos, todo está sujeto a cambios.*
