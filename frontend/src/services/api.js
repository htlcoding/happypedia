// src/services/api.js
// API-Service für die Kommunikation mit dem HappyPedia-Backend
// TODO: Später .env nutzen für die Base-URL

const API_BASE = 'http://localhost:5117/api'

// Alle Artikel laden (sortiert nach Score)
export async function fetchArticles() {
  const res = await fetch(`${API_BASE}/articles`)
  if (!res.ok) throw new Error(`Fehler beim Laden: ${res.status}`)
  return res.json()
}

// Top-Artikel laden
export async function fetchTopArticles(count = 5) {
  const res = await fetch(`${API_BASE}/articles/top?count=${count}`)
  if (!res.ok) throw new Error(`Fehler beim Laden: ${res.status}`)
  return res.json()
}

// Einzelnen Artikel laden
export async function fetchArticle(id) {
  const res = await fetch(`${API_BASE}/articles/${id}`)
  if (!res.ok) throw new Error(`Artikel nicht gefunden: ${res.status}`)
  return res.json()
}

// Testdaten einfügen (Seed)
export async function seedArticles() {
  const res = await fetch(`${API_BASE}/articles/seed`, { method: 'POST' })
  if (!res.ok) throw new Error(`Seed fehlgeschlagen: ${res.status}`)
  return res.text()
}