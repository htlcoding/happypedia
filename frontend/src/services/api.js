<<<<<<< HEAD
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
=======
// src/services/api.js
// API-Service für die Kommunikation mit dem HappyPedia-Backend.
//
// Im Dev-Betrieb wird via Vite-Proxy (siehe vite.config.js) alles unter
// "/api" an http://localhost:5227 weitergeleitet. Für einen anderen
// Backend-Host kann VITE_API_BASE in einer .env-Datei gesetzt werden.

const API_BASE = import.meta.env.VITE_API_BASE ?? '/api'

async function request(path, options = {}) {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {})
    },
    ...options
  })

  if (!res.ok) {
    let message = `HTTP ${res.status}`
    try {
      const body = await res.json()
      if (body?.message) message = body.message
    } catch { /* ignore */ }
    throw new Error(message)
  }

  // 204 oder leere Antwort
  if (res.status === 204) return null
  const text = await res.text()
  if (!text) return null
  try { return JSON.parse(text) } catch { return text }
}

// ───── Artikel ─────────────────────────────────────────────

// GET /api/articles  – alle Artikel (sortiert nach Score)
export function fetchArticles() {
  return request('/articles')
}

// POST /api/articles/fetch – RSS-Feeds neu einlesen
export function fetchArticlesFromFeeds() {
  return request('/articles/fetch', { method: 'POST' })
}

// POST /api/articles/{id}/like
export function likeArticle(id) {
  return request(`/articles/${id}/like`, { method: 'POST' })
}

// POST /api/articles/{id}/dislike
export function dislikeArticle(id) {
  return request(`/articles/${id}/dislike`, { method: 'POST' })
}

// DELETE /api/articles/{id}
export function deleteArticle(id) {
  return request(`/articles/${id}`, { method: 'DELETE' })
}

// ───── RSS-Feeds ───────────────────────────────────────────

// GET /api/rssfeeds
export function fetchRssFeeds() {
  return request('/rssfeeds')
}

// POST /api/rssfeeds – einzelnen Feed anlegen
export function addRssFeed(feed) {
  return request('/rssfeeds', {
    method: 'POST',
    body: JSON.stringify(feed)
  })
}

// POST /api/rssfeeds/bulk – mehrere Feeds anlegen
export function bulkAddRssFeeds(feeds) {
  return request('/rssfeeds/bulk', {
    method: 'POST',
    body: JSON.stringify({ feeds })
  })
}
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
