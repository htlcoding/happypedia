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
