<script setup>
import { ref, onMounted, computed } from 'vue'
import { fetchArticles, seedArticles } from './services/api'

// ── State ──────────────────────────────────────────────
const articles = ref([])
const loading = ref(true)
const error = ref(null)

// ── Hardcoded Fallback-Daten (bis Backend läuft) ──────
const fallbackArticles = [
  {
    id: 1,
    title: 'Wiener Startup entwickelt App gegen Lebensmittelverschwendung',
    url: 'https://example.com/wiener-startup-food',
    source: 'derstandard.at',
    publishedAt: '2026-03-16T10:00:00Z',
    score: 92,
  },
  {
    id: 2,
    title: 'Neue Studie: Ehrenamt stärkt mentale Gesundheit nachweislich',
    url: 'https://example.com/ehrenamt-studie',
    source: 'zeit.de',
    publishedAt: '2026-03-15T08:30:00Z',
    score: 88,
  },
  {
    id: 3,
    title: 'Solarenergie-Rekord in Deutschland: 60% des Stroms aus Erneuerbaren',
    url: 'https://example.com/solar-rekord',
    source: 'tagesschau.de',
    publishedAt: '2026-03-16T14:00:00Z',
    score: 95,
  },
  {
    id: 4,
    title: 'Schweizer Forscher finden neuen Ansatz gegen Antibiotikaresistenz',
    url: 'https://example.com/antibiotika-forschung',
    source: 'nzz.ch',
    publishedAt: '2026-03-14T12:00:00Z',
    score: 85,
  },
  {
    id: 5,
    title: 'Community Gardens in Graz: Nachbarschaft wächst zusammen',
    url: 'https://example.com/community-gardens-graz',
    source: 'kleinezeitung.at',
    publishedAt: '2026-03-17T06:00:00Z',
    score: 78,
  },
  {
    id: 6,
    title: 'Berliner Schüler gewinnen internationalen Robotik-Wettbewerb',
    url: 'https://example.com/robotik-berlin',
    source: 'spiegel.de',
    publishedAt: '2026-03-15T16:00:00Z',
    score: 82,
  },
  {
    id: 7,
    title: 'Neues Bildungsprogramm macht Coding für alle Altersgruppen zugänglich',
    url: 'https://example.com/coding-bildung',
    source: 'golem.de',
    publishedAt: '2026-03-13T09:00:00Z',
    score: 74,
  },
  {
    id: 8,
    title: 'Österreich investiert Milliarden in Bahnausbau für klimafreundliches Reisen',
    url: 'https://example.com/bahn-ausbau-at',
    source: 'orf.at',
    publishedAt: '2026-03-17T12:00:00Z',
    score: 90,
  },
]

// ── API-Aufruf ────────────────────────────────────────
onMounted(async () => {
  try {
    const data = await fetchArticles()
    articles.value = data.length > 0 ? data : fallbackArticles
  } catch (e) {
    console.warn('Backend nicht erreichbar, nutze Fallback-Daten:', e)
    error.value = 'Backend nicht erreichbar – zeige Platzhalter-Daten.'
    articles.value = fallbackArticles
  } finally {
    loading.value = false
  }
})

// ── Computed: Aufteilen in Featured / Main / Sidebar ──
const featuredArticle = computed(() => {
  if (articles.value.length === 0) return null
  // Höchster Score = Featured
  return articles.value[0]
})

const mainArticles = computed(() => {
  return articles.value.slice(1, 4)
})

const sidebarArticles = computed(() => {
  return articles.value.slice(4, 8)
})

// ── Hilfsfunktionen ───────────────────────────────────
function formatDate(isoDate) {
  const d = new Date(isoDate)
  return d.toLocaleDateString('de-AT', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

function formatDateLong() {
  const now = new Date()
  return now.toLocaleDateString('de-AT', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}

// Platzhalter-Bild basierend auf Source
function getPlaceholderImg(source, size) {
  const dims = {
    featured: '860x420',
    card: '520x280',
    sidebar: '90x70',
  }
  // Farbe je nach Source
  const colors = {
    featured: 'c8e6c9/388e3c',
    card: 'e8f5e9/4caf50',
    sidebar: 'e8f5e9/4caf50',
  }
  const label = encodeURIComponent(source)
  return `https://placehold.co/${dims[size]}/${colors[size]}?text=${label}`
}

// Score → Tag-Kategorie (Platzhalter bis echte Kategorien kommen)
function getTagForArticle(article) {
  if (article.score >= 90) return 'Top Story'
  if (article.score >= 80) return 'Empfohlen'
  if (article.score >= 70) return 'Lesenswert'
  return 'Aktuell'
}

// ── Kategorien (statisch) ─────────────────────────────
const categories = [
  { icon: '🌍', name: 'Welt' },
  { icon: '🇦🇹', name: 'Österreich' },
  { icon: '🇩🇪', name: 'Deutschland' },
  { icon: '🇨🇭', name: 'Schweiz' },
  { icon: '💡', name: 'Innovation' },
  { icon: '❤️', name: 'Gesellschaft' },
]

// ── Seed-Button (Dev-Hilfe) ───────────────────────────
const seeding = ref(false)
async function handleSeed() {
  seeding.value = true
  try {
    const msg = await seedArticles()
    alert(msg)
    // Reload
    const data = await fetchArticles()
    articles.value = data
  } catch (e) {
    alert('Seed fehlgeschlagen: ' + e.message)
  } finally {
    seeding.value = false
  }
}
</script>

<template>
  <div class="hp-app">

    <!-- TOP BAR -->
    <div class="hp-topbar">
      <div class="container d-flex justify-content-between align-items-center">
        <span>Deine Plattform für positive Nachrichten aus der gesamten Welt!</span>
        <span>{{ formatDateLong() }}</span>
      </div>
    </div>

    <!-- HEADER -->
    <header class="hp-header">
      <div class="container text-center py-3">
        <a href="#" class="hp-brand text-decoration-none">
          <span class="hp-brand-badge">Happy</span><span class="hp-brand-pedia">Pedia</span>
        </a>
        <p class="hp-brand-claim">Die Happy News Plattform</p>
      </div>
    </header>

    <!-- NAVBAR -->
    <nav class="hp-navbar sticky-top">
      <div class="container">
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
          <div class="d-flex flex-wrap">
            <a v-for="cat in categories" :key="cat.name" href="#" class="hp-nav-item">
              {{ cat.icon }} {{ cat.name }}
            </a>
          </div>
          <div class="d-flex align-items-center gap-2">
            <input type="text" class="hp-search" placeholder="🔍 Suchen..." />
            <a href="#" class="btn hp-btn-sm">Anmelden</a>
          </div>
        </div>
      </div>
    </nav>

    <!-- LOADING STATE -->
    <div v-if="loading" class="container py-5 text-center">
      <p>Lade Artikel...</p>
    </div>

    <!-- MAIN CONTENT -->
    <main v-else class="container py-4">

      <!-- Dev-Hinweis bei Fallback -->
      <div v-if="error" class="alert alert-warning d-flex justify-content-between align-items-center mb-3">
        <span>⚠️ {{ error }}</span>
        <button class="btn btn-sm btn-outline-success" :disabled="seeding" @click="handleSeed">
          {{ seeding ? 'Seeding...' : '🌱 Seed Testdaten' }}
        </button>
      </div>

      <div class="row g-4">

        <!-- LEFT COLUMN -->
        <div class="col-lg-8">

          <!-- FEATURED ARTICLE -->
          <article v-if="featuredArticle" class="hp-featured mb-4">
            <a :href="featuredArticle.url" target="_blank" class="hp-featured-wrap">
              <img
                :src="getPlaceholderImg(featuredArticle.source, 'featured')"
                :alt="featuredArticle.title"
                class="hp-featured-img"
              />
              <div class="hp-featured-overlay">
                <span class="hp-tag hp-tag-white mb-2 d-inline-block">
                  {{ getTagForArticle(featuredArticle) }}
                </span>
                <h1 class="hp-featured-title">{{ featuredArticle.title }}</h1>
                <div class="hp-meta">
                  <span>📰 {{ featuredArticle.source }}</span>
                  <span class="hp-dot">·</span>
                  <span>{{ formatDate(featuredArticle.publishedAt) }}</span>
                  <span class="hp-dot">·</span>
                  <span>⭐ Score: {{ featuredArticle.score }}</span>
                </div>
              </div>
            </a>
          </article>

          <!-- SECTION HEADING -->
          <div class="hp-section-head mb-3">
            <h2 class="hp-section-title">Aktuelle Happy News</h2>
          </div>

          <!-- ARTICLE GRID -->
          <div class="row g-3">
            <div v-for="art in mainArticles" :key="art.id" class="col-sm-6 col-lg-4">
              <article class="hp-card h-100">
                <a :href="art.url" target="_blank">
                  <img
                    :src="getPlaceholderImg(art.source, 'card')"
                    :alt="art.title"
                    class="hp-card-img"
                  />
                </a>
                <div class="hp-card-body">
                  <div class="mb-2">
                    <span class="hp-tag">{{ getTagForArticle(art) }}</span>
                    <span class="hp-tag hp-tag-outline ms-1">{{ art.source }}</span>
                  </div>
                  <h3 class="hp-card-title">
                    <a :href="art.url" target="_blank">{{ art.title }}</a>
                  </h3>
                  <div class="hp-meta mt-2">
                    <span>{{ formatDate(art.publishedAt) }}</span>
                    <span class="hp-dot">·</span>
                    <span>⭐ {{ art.score }}</span>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <div class="text-center mt-4">
            <a href="#" class="btn hp-btn-outline px-5">Weitere Artikel laden</a>
          </div>
        </div>

        <!-- SIDEBAR -->
        <aside class="col-lg-4">

          <!-- NEWSLETTER -->
          <div class="hp-widget hp-widget-green mb-4">
            <h5 class="hp-widget-title">💌 Newsletter</h5>
            <p class="hp-widget-text">Wöchentlich die besten positiven News direkt ins Postfach.</p>
            <input type="email" class="form-control hp-input mb-2" placeholder="deine@email.de" />
            <button class="btn hp-btn-white w-100">Kostenlos anmelden</button>
          </div>

          <!-- KATEGORIEN -->
          <div class="hp-widget mb-4">
            <h5 class="hp-widget-title hp-widget-title-dark">📂 Kategorien</h5>
            <div class="d-flex flex-wrap gap-2 mt-2">
              <a v-for="cat in categories" :key="cat.name" href="#" class="hp-tag hp-tag-link">
                {{ cat.icon }} {{ cat.name }}
              </a>
            </div>
          </div>

          <!-- MEIST GELESEN / TOP SCORED -->
          <div class="hp-widget mb-4">
            <h5 class="hp-widget-title hp-widget-title-dark">🔥 Höchster Score</h5>
            <div class="hp-sidebar-list">
              <a
                v-for="(art, i) in sidebarArticles"
                :key="art.id"
                :href="art.url"
                target="_blank"
                class="hp-sidebar-item"
              >
                <span class="hp-sidebar-num">{{ i + 1 }}</span>
                <img
                  :src="getPlaceholderImg(art.source, 'sidebar')"
                  :alt="art.title"
                  class="hp-sidebar-img"
                />
                <div class="hp-sidebar-info">
                  <span class="hp-tag hp-tag-xs">⭐ {{ art.score }}</span>
                  <p class="hp-sidebar-title">{{ art.title }}</p>
                  <small class="hp-sidebar-date">{{ art.source }} · {{ formatDate(art.publishedAt) }}</small>
                </div>
              </a>
            </div>
          </div>

        </aside>
      </div>
    </main>

    <!-- FOOTER -->
    <footer class="hp-footer">
      <div class="container">
        <div class="row g-4 py-5">
          <div class="col-lg-4">
            <a href="#" class="hp-brand text-decoration-none">
              <span class="hp-brand-badge">Happy</span><span class="hp-brand-pedia" style="color:#a5d6a7">Pedia</span>
            </a>
            <p class="hp-footer-text mt-2">Positive Nachrichten aus dem DACH-Raum und der Welt.</p>
          </div>
          <div class="col-6 col-lg-2">
            <h6 class="hp-footer-heading">Regionen</h6>
            <ul class="list-unstyled hp-footer-links">
              <li><a href="#">Österreich</a></li>
              <li><a href="#">Deutschland</a></li>
              <li><a href="#">Schweiz</a></li>
              <li><a href="#">International</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <h6 class="hp-footer-heading">Über uns</h6>
            <ul class="list-unstyled hp-footer-links">
              <li><a href="#">Team</a></li>
              <li><a href="#">Kontakt</a></li>
              <li><a href="#">Quellen</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <h6 class="hp-footer-heading">Rechtliches</h6>
            <ul class="list-unstyled hp-footer-links">
              <li><a href="#">Impressum</a></li>
              <li><a href="#">Datenschutz</a></li>
              <li><a href="#">AGB</a></li>
            </ul>
          </div>
        </div>
        <div class="hp-footer-bottom d-flex justify-content-between align-items-center py-3">
          <small class="hp-footer-text">© 2026 HappyPedia. Alle Rechte vorbehalten.</small>
          <div class="d-flex gap-3">
            <a href="#" class="hp-social">📘</a>
            <a href="#" class="hp-social">📸</a>
            <a href="#" class="hp-social">🐦</a>
          </div>
        </div>
      </div>
    </footer>

  </div>
</template>