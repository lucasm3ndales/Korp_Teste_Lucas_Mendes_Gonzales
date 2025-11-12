import { Injectable } from '@angular/core';
import { MessageService, ToastMessageOptions } from 'primeng/api';
import {IApiResult} from '../../../shared/models/default.model';


@Injectable({
  providedIn: 'root',
})
export class ToastService {

  constructor(private messageService: MessageService) { }

  public showToast(
    severity: 'success' | 'info' | 'warn' | 'error',
    summary: string,
    detail: string,
    life: number = 3000,
    key?: string
  ): void {

    const message: ToastMessageOptions = {
      severity: severity,
      summary: summary,
      detail: detail,
      key: key,
      life: life,
    };

    this.messageService.add(message);
  }

  public showSuccess(summary: string, detail: string = 'Operação concluída com sucesso.', life: number = 3000): void {
    this.showToast('success', summary, detail, life);
  }

  public showError(summary: string, detail: string = 'Ocorreu um erro. Tente novamente mais tarde.', life: number = 4000): void {
    this.showToast('error', summary, detail, life);
  }

  public showInfo(summary: string, detail: string = 'Informação importante.', life: number = 3000): void {
    this.showToast('info', summary, detail, life);
  }

  public showWarning(
    summary: string,
    detail: string = 'Atenção! Verifique as informações antes de continuar.',
    life: number = 3000
  ): void {
    this.showToast('warn', summary, detail, life);
  }

  public showApiResult<T>(
    result: IApiResult<T>,
    successSummary: string = 'Sucesso',
    errorSummary: string = 'Erro na Operação'
  ): void {
    const message = result.messages && result.messages.length > 0
      ? result.messages[0]
      : '';

    if (result.isSuccess) {
      if (message) {
        this.showSuccess(successSummary, message);
      } else {
        this.showSuccess(successSummary);
      }
    } else {
      if (message) {
        this.showError(errorSummary, message);
      } else {
        this.showError(errorSummary);
      }
    }
  }

  public showApiSuccess<T>(result: IApiResult<T>, summary: string = 'Sucesso'): void {
    const detail = result.messages && result.messages.length > 0
      ? result.messages[0]
      : undefined;

    this.showSuccess(summary, detail);
  }

  public showApiError<T>(result: IApiResult<T>, summary: string = 'Erro na Operação'): void {
    const detail = result.messages && result.messages.length > 0
      ? result.messages[0]
      : undefined;
    this.showError(summary, detail);
  }
}
