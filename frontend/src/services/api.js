const API_BASE = 'http://localhost:5227/api'

function getUserId() {
  const user = JSON.parse(localStorage.getItem('hp_user') || 'null')
  return user?.id
}

async function request(path, options = {}) {
  const userId = getUserId()

  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(userId ? { 'X-User-Id': userId } : {}),
      ...(options.headers ?? {})
    }
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `Fehler: ${res.status}`)
  }

  return res.json()
}

export function fetchArticles() {
  return request('/articles')
}

export function fetchArticlesFromFeeds() {
  return request('/articles/fetch', { method: 'POST' })
}

export function likeArticle(id) {
  return request(`/articles/${id}/like`, { method: 'POST' })
}

export function dislikeArticle(id) {
  return request(`/articles/${id}/dislike`, { method: 'POST' })
}

export function deleteArticle(id) {
  return request(`/articles/${id}`, { method: 'DELETE' })
}

export function login(username, password) {
  return request('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password })
  })
}

export function register(username, password) {
  return request('/auth/register', {
    method: 'POST',
    body: JSON.stringify({ username, password })
  })
}

export function fetchComments(articleId) {
  return request(`/articles/${articleId}/comments`)
}

export function addComment(articleId, text) {
  return request(`/articles/${articleId}/comments`, {
    method: 'POST',
    body: JSON.stringify({ text })
  })
}