# ROV All Blue

## **Os seguintes itens tem que estar neste repositório**
# Estrutura de Classes do ROV Educacional

## 1. Motores

### Motor
**Atributos:**
- `pino` (int) → Define o endereço de controle do motor.
- `velocidade` (int) → Define a velocidade do motor.

**Métodos:**
- `acionarMotor()` → Aciona o motor.
- `defVel()` → Define a velocidade do motor.

### Servo
**Atributos:**
- `address` (string) → Define o endereço de controle do servo.
- `posição` (float) → Define a posição do servo.

**Métodos:**
- `acionarServo()` → Aciona o servo.
- `defPos()` → Define a posição do servo.

---

## 2. Controle

### Movimentação
**Atributos:**
- `esquerdo` (Motor) → Motor do lado esquerdo.
- `direito` (Motor) → Motor do lado direito.
- `servoSeringa` (Servo) → Servo utilizado para controle.
- `altura` (vetor) → Define a altura.
- `orientação` (vetor) → Define a orientação do ROV.

**Métodos:**
- `frente()` → Movimenta para frente.
- `re()` → Movimenta para trás.
- `virarDireita()` → Faz o ROV virar à direita.
- `virarEsquerda()` → Faz o ROV virar à esquerda.
- `submergir()` → Permite o ROV submergir.
- `mergulhar()` → Controla a profundidade.
- `neutro()` → Retorna o ROV ao estado neutro.

---

## 3. Data

### Data
**Atributos:**
- `temperatura` (Sensor) → Sensor de temperatura.
- `pressao` (Sensor) → Sensor de pressão.
- `laser` (Sensor) → Sensor a laser.
- `vazao` (Sensor) → Sensor de vazão.

**Métodos:**
- `mandarDados()` → Envia os dados coletados.
- `getAltura()` → Retorna a altura do ROV.
- `getOrientacao()` → Retorna a orientação do ROV.

### Sensor
**Atributos:**
- `address` (string) → Endereço do sensor no barramento.
- `nome` (string) → Nome do sensor.
- `valor` (double) → Valor medido
