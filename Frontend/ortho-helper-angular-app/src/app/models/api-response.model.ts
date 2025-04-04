// src/app/models/api-response.model.ts
export interface ApiResponse {
  inputText: string;
  outputText: string;
  diff: string;
  processingTime: string;
  timestamp: string;
}
