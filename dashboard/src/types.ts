export type CompareResult = {
  expression: string
  bruteForceResult: number
  optimizedResult: number
  match: boolean
}

export type EvaluationStep = { stepNumber: number; description: string; state: string }

export type TraceResult = {
  expression: string
  bruteForceSteps: EvaluationStep[]
  optimizedSteps: EvaluationStep[]
  result: number
}

export type ApiError = { detail?: string; title?: string }
