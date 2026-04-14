const API_BASE = 'https://localhost:7045/api/'

export async function fetchArticles() {
  const res = await fetch(`${API_BASE}articles`)
  if (!res.ok) throw new Error(`Fehler beim Laden der Artikel: ${res.status}`)
  return res.json()
}
export async function fetchArticlesFromFeeds() {
  const res = await fetch(`${API_BASE}articles/fetch`, {
    method: 'POST'
  })
  if (!res.ok) throw new Error(`Fehler beim Fetchen der Artikel: ${res.status}`)
  return res.json()
}

export async function likeArticle(id) {
  const res = await fetch(`${API_BASE}articles/${id}/like`, {
    method: 'POST'
  })
  if (!res.ok) throw new Error(`Fehler beim Liken: ${res.status}`)
  return res.json()
}

export async function dislikeArticle(id) {
  const res = await fetch(`${API_BASE}articles/${id}/dislike`, {
    method: 'POST'
  })
  if (!res.ok) throw new Error(`Fehler beim Disliken: ${res.status}`)
  return res.json()
}

export async function deleteArticle(id) {
  const res = await fetch(`${API_BASE}articles/${id}`, {
    method: 'DELETE'
  })
  if (!res.ok) throw new Error(`Fehler beim Löschen: ${res.status}`)
  return res.json()
}