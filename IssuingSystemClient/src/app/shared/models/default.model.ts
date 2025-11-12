

export interface IApiResult<T> {
  isSuccess: boolean;
  messages: string[];
  data?: T
}
