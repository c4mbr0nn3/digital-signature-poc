export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl || 'http://localhost:5071/api/v1'

  const apiFetch = async <T>(
    endpoint: string,
    options: Parameters<typeof $fetch>[1] = {}
  ): Promise<{ data: T | null; error: string | null }> => {
    try {
      const data = await $fetch<T>(endpoint, {
        baseURL,
        ...options,
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
      })

      return { data, error: null }
    } catch (error: any) {
      console.error('API Error:', error)

      const errorMessage =
        error?.data?.message ||
        error?.message ||
        error?.statusMessage ||
        'Unknown error'

      return {
        data: null,
        error: errorMessage,
      }
    }
  }

  const get = <T>(endpoint: string, options: Parameters<typeof $fetch>[1] = {}) => {
    return apiFetch<T>(endpoint, { ...options, method: 'GET' })
  }

  const post = <T>(endpoint: string, body?: any, options: Parameters<typeof $fetch>[1] = {}) => {
    return apiFetch<T>(endpoint, { ...options, method: 'POST', body })
  }

  const put = <T>(endpoint: string, body?: any, options: Parameters<typeof $fetch>[1] = {}) => {
    return apiFetch<T>(endpoint, { ...options, method: 'PUT', body })
  }

  const patch = <T>(endpoint: string, body?: any, options: Parameters<typeof $fetch>[1] = {}) => {
    return apiFetch<T>(endpoint, { ...options, method: 'PATCH', body })
  }

  const del = <T>(endpoint: string, options: Parameters<typeof $fetch>[1] = {}) => {
    return apiFetch<T>(endpoint, { ...options, method: 'DELETE' })
  }

  return {
    apiFetch,
    baseURL,
    get,
    post,
    put,
    patch,
    del,
  }
}