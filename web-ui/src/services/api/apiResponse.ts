export class ApiResponse {
  kind:
    | 'ok'
    | 'unknown'
    | 'not-found'
    | 'forbidden'
    | 'bad-data'
    | 'server'
    | 'NetworkError'
    | '' = '';

  data?: any;
  statusText?: any;

  temporary?: any;

  constructor(
    kind:
      | 'ok'
      | 'unknown'
      | 'not-found'
      | 'forbidden'
      | 'bad-data'
      | 'server'
      | 'NetworkError',
    statusText?: any,
    data?: any,
    temporary?: any
  ) {
    this.kind = kind;
    this.data = data;
    this.statusText = statusText;

    this.temporary = temporary;
  }
}
