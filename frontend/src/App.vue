<script setup>
import { ref, onMounted, computed } from 'vue'
import {
  fetchArticles,
  fetchArticlesFromFeeds,
  likeArticle,
  dislikeArticle,
} from './services/api'

// ─────────────────────────────────────────────────────────────
//  Logo-Assets
//  Lege folgende Dateien in src/assets/ ab:
//    happypedia-logo.png  (das volle Wortmarken-Logo)
//    happypedia-icon.png  (nur das Smiley-Icon, optional)
// ─────────────────────────────────────────────────────────────
import logoUrl from './assets/happypedia-logo.png'
import iconUrl from './assets/happypedia-icon.png'

// Backend-Artikel (description, imageUrl, upvotes/downvotes) in das
// vom Template erwartete Format bringen.
function normalizeArticle(a) {
  return {
    ...a,
    summary: a.summary ?? a.description ?? '',
  }
}

const articles        = ref([])
const loading         = ref(true)
const error           = ref(null)
const selectedArticle = ref(null)
const commentInput    = ref('')
const darkMode        = ref(false)

const likes    = ref({})
const liked    = ref({})
const comments = ref({})

const fallbackArticles = [
  {
    id: 1,
    title: 'Wiener Startup entwickelt App gegen Lebensmittelverschwendung',
    url: 'https://example.com/wiener-startup-food',
    source: 'derstandard.at',
    category: 'Innovation',
    publishedAt: '2026-03-16T10:00:00Z',
    score: 92,
    summary: 'Ein junges Wiener Startup hat eine App entwickelt, die Supermärkte und Restaurants dabei hilft, überschüssige Lebensmittel an Bedürftige weiterzugeben – und damit Tonnen von Lebensmittelabfällen zu vermeiden.',
  },
  {
    id: 2,
    title: 'Neue Studie: Ehrenamt stärkt mentale Gesundheit nachweislich',
    url: 'https://example.com/ehrenamt-studie',
    source: 'zeit.de',
    category: 'Gesellschaft',
    publishedAt: '2026-03-15T08:30:00Z',
    score: 88,
    summary: 'Eine großangelegte Studie mit über 10.000 Teilnehmern belegt: Wer regelmäßig ehrenamtlich tätig ist, berichtet signifikant seltener von Burnout, Einsamkeit und Depressionen.',
  },
  {
    id: 3,
    title: 'Solarenergie-Rekord in Deutschland: 60% des Stroms aus Erneuerbaren',
    url: 'https://example.com/solar-rekord',
    source: 'tagesschau.de',
    category: 'Klima',
    publishedAt: '2026-03-16T14:00:00Z',
    score: 95,
    summary: 'Deutschland hat einen neuen Rekord bei erneuerbaren Energien aufgestellt: An einem sonnigen Frühlingstag deckten Solar-, Wind- und Wasserkraft gemeinsam 60 % des gesamten Strombedarfs.',
  },
  {
    id: 4,
    title: 'Schweizer Forscher finden neuen Ansatz gegen Antibiotikaresistenz',
    url: 'https://example.com/antibiotika-forschung',
    source: 'nzz.ch',
    category: 'Wissenschaft',
    publishedAt: '2026-03-14T12:00:00Z',
    score: 85,
    summary: 'Forschende der ETH Zürich haben einen molekularen Mechanismus entdeckt, der resistente Bakterien wieder für bestehende Antibiotika empfänglich machen könnte – ein Durchbruch für die Medizin.',
  },
  {
    id: 5,
    title: 'Community Gardens in Graz: Nachbarschaft wächst zusammen',
    url: 'https://example.com/community-gardens-graz',
    source: 'kleinezeitung.at',
    category: 'Gesellschaft',
    publishedAt: '2026-03-17T06:00:00Z',
    score: 78,
    summary: 'In Graz entstehen immer mehr Gemeinschaftsgärten, in denen Menschen verschiedener Herkunft gemeinsam anbauen, kochen und feiern. Ein Grazer Verein koordiniert mittlerweile 24 solcher Projekte.',
  },
  {
    id: 6,
    title: 'Berliner Schüler gewinnen internationalen Robotik-Wettbewerb',
    url: 'https://example.com/robotik-berlin',
    source: 'spiegel.de',
    category: 'Bildung',
    publishedAt: '2026-03-15T16:00:00Z',
    score: 82,
    summary: 'Ein Team der Geschwister-Scholl-Oberschule Berlin hat beim World Robot Olympiad den ersten Platz in der Kategorie „Future Engineers" belegt – und damit alle 47 Teilnehmerländer hinter sich gelassen.',
  },
  {
    id: 7,
    title: 'Neues Bildungsprogramm macht Coding für alle Altersgruppen zugänglich',
    url: 'https://example.com/coding-bildung',
    source: 'golem.de',
    category: 'Bildung',
    publishedAt: '2026-03-13T09:00:00Z',
    score: 74,
    summary: 'Das neue Förderprogramm „CodeForAll" bietet kostenlosen Programmierunterricht für Kinder ab 6 Jahren bis hin zu Senioren – und verzeichnet bereits über 50.000 aktive Teilnehmer in Deutschland.',
  },
  {
    id: 8,
    title: 'Österreich investiert Milliarden in Bahnausbau für klimafreundliches Reisen',
    url: 'https://example.com/bahn-ausbau-at',
    source: 'orf.at',
    category: 'Klima',
    publishedAt: '2026-03-17T12:00:00Z',
    score: 90,
    summary: 'Das Klimaschutzministerium hat ein 18-Milliarden-Euro-Paket für den Ausbau des österreichischen Schienennetzes beschlossen – mit neuen Hochgeschwindigkeitstrassen und dichterem Regionalverkehr.',
  },
]

async function loadArticles() {
  try {
    const data = await fetchArticles()
    const normalized = Array.isArray(data) ? data.map(normalizeArticle) : []
    articles.value = normalized.length > 0 ? normalized : fallbackArticles
    if (normalized.length > 0) error.value = null
  } catch (e) {
    console.warn('Backend nicht erreichbar:', e)
    error.value = 'Backend nicht erreichbar – Platzhalter-Daten werden angezeigt.'
    articles.value = fallbackArticles
  }

  const init = (list) => list.forEach(a => {
    likes.value[a.id]    = likes.value[a.id]    ?? (a.upvotes ?? Math.floor(Math.random() * 40 + 5))
    liked.value[a.id]    = liked.value[a.id]    ?? false
    comments.value[a.id] = comments.value[a.id] ?? []
  })
  init(fallbackArticles)
  init(articles.value)
}

onMounted(async () => {
  await loadArticles()
  loading.value = false
})

const sortedArticles    = computed(() => [...articles.value].sort((a, b) => b.score - a.score))
const featuredArticle   = computed(() => sortedArticles.value[0] ?? null)
const secondaryArticle  = computed(() => sortedArticles.value[1] ?? null)
const trioArticles      = computed(() => sortedArticles.value.slice(2, 5))
const restArticles      = computed(() => sortedArticles.value.slice(5))

async function toggleLike(id) {
  const wasLiked = liked.value[id]
  if (wasLiked) { likes.value[id]--; liked.value[id] = false }
  else          { likes.value[id]++; liked.value[id] = true  }

  const article = articles.value.find(a => a.id === id)
  const isBackendArticle = article && typeof article.upvotes === 'number'
  if (!isBackendArticle) return

  try {
    const updated = wasLiked ? await dislikeArticle(id) : await likeArticle(id)
    if (updated) {
      const idx = articles.value.findIndex(a => a.id === id)
      if (idx >= 0) articles.value[idx] = normalizeArticle(updated)
      likes.value[id] = updated.upvotes
    }
  } catch (e) {
    console.warn('Like konnte nicht gespeichert werden:', e)
    if (wasLiked) { likes.value[id]++; liked.value[id] = true }
    else          { likes.value[id]--; liked.value[id] = false }
  }
}

function submitComment(articleId) {
  const text = commentInput.value.trim()
  if (!text) return
  if (!comments.value[articleId]) comments.value[articleId] = []
  comments.value[articleId].push({
    author: 'Du',
    text,
    date: new Date().toLocaleDateString('de-AT', { day: '2-digit', month: '2-digit', year: 'numeric' }),
  })
  commentInput.value = ''
}

// Eigene Kommentare löschen ────────────────────────────────────
function deleteComment(articleId, idx) {
  if (!comments.value[articleId]) return
  const c = comments.value[articleId][idx]
  if (c?.author !== 'Du') return // Sicherheitscheck: nur eigene
  if (!confirm('Diesen Kommentar wirklich löschen?')) return
  comments.value[articleId].splice(idx, 1)
}

function openArticle(article) {
  selectedArticle.value = article
  commentInput.value = ''
  document.body.style.overflow = 'hidden'
}

function closeArticle() {
  selectedArticle.value = null
  document.body.style.overflow = ''
}

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('de-AT', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatDateLong() {
  return new Date().toLocaleDateString('de-AT', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
}

function scoreBadge(score) {
  if (score >= 90) return 'Top Story'
  if (score >= 80) return 'Empfohlen'
  if (score >= 70) return 'Lesenswert'
  return 'Aktuell'
}

// Bild eines Artikels: Backend liefert imageUrl, Fallback nutzt Picsum
function articleImage(article) {
  if (article?.imageUrl) return article.imageUrl
  return `https://picsum.photos/seed/hp${article?.id ?? 0}/1200/800`
}

const categories = [
  { label: 'Welt' },
  { label: 'Österreich' },
  { label: 'Deutschland' },
  { label: 'Schweiz' },
  { label: 'Innovation' },
  { label: 'Gesellschaft' },
]

const seeding = ref(false)
async function handleSeed() {
  seeding.value = true
  try {
    const result = await fetchArticlesFromFeeds()
    const imported = result?.imported ?? 0
    alert(`${imported} Artikel aus den RSS-Feeds importiert.`)
    await loadArticles()
  } catch (e) {
    alert('Artikel-Import fehlgeschlagen: ' + e.message)
  } finally {
    seeding.value = false
  }
}

function toggleTheme() {
  darkMode.value = !darkMode.value
}
</script>

<template>
  <div class="app" :class="{ 'theme-dark': darkMode }">

    <!-- ════════════════════ HEADER ════════════════════ -->
    <header class="header">
      <div class="header-inner">

        <a href="#" class="logo" aria-label="HappyPedia – Startseite">
          <img :src="logoUrl" alt="HappyPedia" class="logo-img" width="200"/>
        </a>

        <nav class="nav-links">
          <a v-for="cat in categories" :key="cat.label" href="#" class="nav-link">{{ cat.label }}</a>
        </nav>

        <div class="header-actions">
          <button class="icon-btn" aria-label="Suchen">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
              <circle cx="7" cy="7" r="5" stroke="currentColor" stroke-width="1.6" />
              <path d="M11 11l3.5 3.5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" />
            </svg>
          </button>
          <button class="icon-btn theme-toggle" @click="toggleTheme"
                  :aria-label="darkMode ? 'Helles Design' : 'Dunkles Design'">
            <svg v-if="!darkMode" width="18" height="18" viewBox="0 0 18 18" fill="none">
              <circle cx="9" cy="9" r="3.4" fill="currentColor" />
              <g stroke="currentColor" stroke-width="1.4" stroke-linecap="round">
                <line x1="9" y1="1.5" x2="9" y2="3.5" />
                <line x1="9" y1="14.5" x2="9" y2="16.5" />
                <line x1="1.5" y1="9" x2="3.5" y2="9" />
                <line x1="14.5" y1="9" x2="16.5" y2="9" />
                <line x1="3.7" y1="3.7" x2="5.1" y2="5.1" />
                <line x1="12.9" y1="12.9" x2="14.3" y2="14.3" />
                <line x1="12.9" y1="5.1" x2="14.3" y2="3.7" />
                <line x1="3.7" y1="14.3" x2="5.1" y2="12.9" />
              </g>
            </svg>
            <svg v-else width="18" height="18" viewBox="0 0 18 18" fill="none">
              <path d="M14.5 11.5A6 6 0 1 1 6.5 3.5a6.5 6.5 0 1 0 8 8z" fill="currentColor" />
            </svg>
          </button>
          <a href="#" class="btn-login">Anmelden</a>
        </div>
      </div>
    </header>

    <!-- ════════════════════ LOADING ════════════════════ -->
    <div v-if="loading" class="loading-state">
      <div class="spinner" />
      <p>Artikel werden geladen …</p>
    </div>

    <!-- ════════════════════ MAIN ════════════════════ -->
    <main v-else class="main">

      <div v-if="error" class="error-banner">
        <span>{{ error }}</span>
        <button class="btn-seed" :disabled="seeding" @click="handleSeed">
          {{ seeding ? 'Importiere …' : 'Artikel aus RSS laden' }}
        </button>
      </div>

      <div class="eyebrow">
        <span class="eyebrow-dot" />
        <span class="eyebrow-text">{{ formatDateLong() }}</span>
        <span class="eyebrow-tag">Heute kuratiert</span>
      </div>

      <h1 class="page-headline">
        Gute Nachrichten, <em>klar sortiert.</em>
      </h1>

      <!-- ═════════════ MAGAZINE: Row 1 (groß + mittel) ═════════════ -->
      <section v-if="featuredArticle" class="row-hero">

        <article class="mag-card mag-card-big" @click="openArticle(featuredArticle)"
                 role="button" tabindex="0" @keydown.enter="openArticle(featuredArticle)">
          <img :src="articleImage(featuredArticle)" :alt="featuredArticle.title" class="mag-img" />
          <div class="mag-shade" />
          <div class="mag-body">
            <div class="mag-badges">
              <span class="pill pill-cat">{{ featuredArticle.category ?? scoreBadge(featuredArticle.score) }}</span>
            </div>
            <h2 class="mag-title mag-title-big">{{ featuredArticle.title }}</h2>
            <p class="mag-summary">{{ featuredArticle.summary }}</p>
            <div class="mag-foot">
              <span class="mag-date">
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                  <rect x="1.5" y="2.5" width="10" height="9" rx="1.5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="1.5" y1="5" x2="11.5" y2="5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="4" y1="1" x2="4" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                  <line x1="9" y1="1" x2="9" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                </svg>
                {{ formatDate(featuredArticle.publishedAt) }}
              </span>
              <span class="mag-source">{{ featuredArticle.source }}</span>
            </div>
          </div>
        </article>

        <article v-if="secondaryArticle" class="mag-card mag-card-side" @click="openArticle(secondaryArticle)"
                 role="button" tabindex="0" @keydown.enter="openArticle(secondaryArticle)">
          <img :src="articleImage(secondaryArticle)" :alt="secondaryArticle.title" class="mag-img" />
          <div class="mag-shade" />
          <div class="mag-body">
            <div class="mag-badges">
              <span class="pill pill-cat">{{ secondaryArticle.category ?? scoreBadge(secondaryArticle.score) }}</span>
            </div>
            <h3 class="mag-title">{{ secondaryArticle.title }}</h3>
            <div class="mag-foot">
              <span class="mag-date">
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                  <rect x="1.5" y="2.5" width="10" height="9" rx="1.5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="1.5" y1="5" x2="11.5" y2="5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="4" y1="1" x2="4" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                  <line x1="9" y1="1" x2="9" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                </svg>
                {{ formatDate(secondaryArticle.publishedAt) }}
              </span>
            </div>
          </div>
        </article>
      </section>

      <!-- ═════════════ MAGAZINE: Row 2 (3 gleich große Cards) ═════════════ -->
      <section v-if="trioArticles.length" class="row-trio">
        <article v-for="(art, idx) in trioArticles" :key="art.id"
                 class="mag-card mag-card-medium"
                 :style="{ '--delay': `${idx * 70}ms` }"
                 @click="openArticle(art)"
                 role="button" tabindex="0" @keydown.enter="openArticle(art)">
          <img :src="articleImage(art)" :alt="art.title" class="mag-img" />
          <div class="mag-shade" />
          <div class="mag-body">
            <div class="mag-badges">
              <span class="pill pill-cat">{{ art.category ?? scoreBadge(art.score) }}</span>
            </div>
            <h3 class="mag-title">{{ art.title }}</h3>
            <div class="mag-foot">
              <span class="mag-date">
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                  <rect x="1.5" y="2.5" width="10" height="9" rx="1.5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="1.5" y1="5" x2="11.5" y2="5" stroke="currentColor" stroke-width="1.2" />
                  <line x1="4" y1="1" x2="4" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                  <line x1="9" y1="1" x2="9" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                </svg>
                {{ formatDate(art.publishedAt) }}
              </span>
            </div>
          </div>
        </article>
      </section>

      <!-- ═════════════ Weitere Artikel (klassisches Grid) ═════════════ -->
      <template v-if="restArticles.length">
        <div class="section-head">
          <h3 class="section-title">Aktuelle Happy News</h3>
          <a href="#" class="section-link">Alle anzeigen →</a>
        </div>

        <section class="row-rest">
          <article v-for="(art, idx) in restArticles" :key="art.id"
                   class="mag-card mag-card-small"
                   :style="{ '--delay': `${idx * 70}ms` }"
                   @click="openArticle(art)"
                   role="button" tabindex="0" @keydown.enter="openArticle(art)">
            <img :src="articleImage(art)" :alt="art.title" class="mag-img" />
            <div class="mag-shade" />
            <div class="mag-body">
              <div class="mag-badges">
                <span class="pill pill-cat">{{ art.category ?? scoreBadge(art.score) }}</span>
              </div>
              <h4 class="mag-title">{{ art.title }}</h4>
              <div class="mag-foot">
                <span class="mag-date">
                  <svg width="13" height="13" viewBox="0 0 13 13" fill="none">
                    <rect x="1.5" y="2.5" width="10" height="9" rx="1.5" stroke="currentColor" stroke-width="1.2" />
                    <line x1="1.5" y1="5" x2="11.5" y2="5" stroke="currentColor" stroke-width="1.2" />
                    <line x1="4" y1="1" x2="4" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                    <line x1="9" y1="1" x2="9" y2="3.5" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" />
                  </svg>
                  {{ formatDate(art.publishedAt) }}
                </span>
              </div>
            </div>
          </article>
        </section>
      </template>

      <div class="load-more-wrap">
        <a href="#" class="btn-load-more">
          <span>Weitere Artikel laden</span>
          <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
            <path d="M3 7h8M7 3l4 4-4 4" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
        </a>
      </div>
    </main>

    <!-- ════════════════════ FOOTER ════════════════════ -->
    <footer class="footer">
      <div class="footer-inner">
        <div class="footer-brand">
          <a href="#" class="logo">
            <img :src="logoUrl" alt="HappyPedia" class="logo-img" />
          </a>
          <p class="footer-desc">Positive Nachrichten aus dem DACH-Raum und der Welt – kuratiert, gewichtet, sortiert.</p>
        </div>
        <div class="footer-col">
          <h6>Regionen</h6>
          <a href="#">Österreich</a>
          <a href="#">Deutschland</a>
          <a href="#">Schweiz</a>
          <a href="#">International</a>
        </div>
        <div class="footer-col">
          <h6>Über uns</h6>
          <a href="#">Team</a>
          <a href="#">Kontakt</a>
          <a href="#">Quellen</a>
        </div>
        <div class="footer-col">
          <h6>Rechtliches</h6>
          <a href="#">Impressum</a>
          <a href="#">Datenschutz</a>
          <a href="#">AGB</a>
        </div>
      </div>
      <div class="footer-bottom">
        <small>© 2026 HappyPedia · Made with care in Wien</small>
      </div>
    </footer>

    <!-- ════════════════════ DETAIL MODAL ════════════════════ -->
    <Transition name="hp-fade">
      <div v-if="selectedArticle" class="hp-overlay" @click.self="closeArticle">
        <div class="hp-dialog" role="dialog" aria-modal="true" @click.stop>

          <div class="hp-dialog-head">
            <div class="hp-dialog-head-badges">
              <span class="pill pill-cat">{{ selectedArticle.category ?? scoreBadge(selectedArticle.score) }}</span>
              <span class="pill pill-outline">{{ selectedArticle.source }}</span>
            </div>
            <button class="hp-dialog-close" @click="closeArticle" aria-label="Schließen">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
                <path d="M3 3l10 10M13 3L3 13" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              </svg>
            </button>
          </div>

          <div class="hp-dialog-scroll">
            <div class="hp-dialog-media">
              <img :src="articleImage(selectedArticle)" :alt="selectedArticle.title" class="hp-dialog-img" />
            </div>

            <div class="hp-dialog-content">
              <h2 class="hp-dialog-title">{{ selectedArticle.title }}</h2>
              <div class="meta hp-dialog-meta">
                <span>{{ selectedArticle.source }}</span>
                <span class="sep" />
                <span>{{ formatDate(selectedArticle.publishedAt) }}</span>
                <span class="sep" />
                <span>Score {{ selectedArticle.score }}</span>
              </div>

              <p class="hp-dialog-summary">{{ selectedArticle.summary ?? 'Kein Kurztext verfügbar.' }}</p>

              <a :href="selectedArticle.url" target="_blank" rel="noopener" class="btn-original">
                <span>Originalartikel öffnen</span>
                <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                  <path d="M5 2h7v7M12 2L2 12" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
              </a>
            </div>

            <!-- ═══════ LIKE-BEREICH ═══════ -->
            <section class="hp-section hp-section-like">
              <header class="hp-section-head">
                <span class="hp-section-icon hp-section-icon-like">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor"><path d="M12 21s-7.5-4.5-9.5-9C1.1 8.7 3.4 5 7 5c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3.6 0 5.9 3.7 4.5 7-2 4.5-9.5 9-9.5 9z"/></svg>
                </span>
                <div class="hp-section-headtext">
                  <h3 class="hp-section-title">Gefällt dir der Artikel?</h3>
                  <p class="hp-section-sub">Mit deinem Like hilfst du, gute Nachrichten weiter oben zu zeigen.</p>
                </div>
              </header>

              <div class="like-stat">
                <button class="like-btn" :class="{ active: liked[selectedArticle.id] }"
                        @click="toggleLike(selectedArticle.id)"
                        :aria-pressed="liked[selectedArticle.id]">
                  <svg class="like-icon" width="22" height="22" viewBox="0 0 24 24" fill="none">
                    <path d="M12 21s-7.5-4.5-9.5-9C1.1 8.7 3.4 5 7 5c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3.6 0 5.9 3.7 4.5 7-2 4.5-9.5 9-9.5 9z"
                          :fill="liked[selectedArticle.id] ? 'currentColor' : 'none'"
                          stroke="currentColor" stroke-width="1.8" stroke-linejoin="round" />
                  </svg>
                  <span>{{ liked[selectedArticle.id] ? 'Geliked' : 'Like' }}</span>
                </button>

                <div class="like-count">
                  <span class="like-count-num">{{ likes[selectedArticle.id] ?? 0 }}</span>
                  <span class="like-count-lbl">Likes insgesamt</span>
                </div>
              </div>
            </section>

            <div class="hp-divider">
              <span class="hp-divider-line" />
              <span class="hp-divider-text">Diskussion</span>
              <span class="hp-divider-line" />
            </div>

            <!-- ═══════ KOMMENTAR-BEREICH ═══════ -->
            <section class="hp-section hp-section-comments">
              <header class="hp-section-head">
                <span class="hp-section-icon hp-section-icon-cmt">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
                    <path d="M21 12a8 8 0 0 1-11.5 7.2L4 21l1.8-5.5A8 8 0 1 1 21 12z"
                          stroke="currentColor" stroke-width="2" stroke-linejoin="round" fill="currentColor" fill-opacity=".15" />
                  </svg>
                </span>
                <div class="hp-section-headtext">
                  <h3 class="hp-section-title">
                    Kommentare
                    <span class="hp-count-pill">{{ (comments[selectedArticle.id] ?? []).length }}</span>
                  </h3>
                  <p class="hp-section-sub">Bleib respektvoll – konstruktive Beiträge sind willkommen.</p>
                </div>
              </header>

              <div class="comment-form">
                <div class="comment-form-avatar">Du</div>
                <div class="comment-form-input">
                  <textarea
                    v-model="commentInput"
                    class="comment-input"
                    placeholder="Schreib deinen Kommentar … (Strg+Enter zum Senden)"
                    rows="3"
                    maxlength="500"
                    @keydown.ctrl.enter="submitComment(selectedArticle.id)"
                    @keydown.meta.enter="submitComment(selectedArticle.id)"
                  ></textarea>
                  <div class="comment-form-actions">
                    <span class="comment-form-hint">{{ commentInput.length }}/500</span>
                    <button class="btn-send" :disabled="!commentInput.trim()" @click="submitComment(selectedArticle.id)">
                      <span>Senden</span>
                      <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                        <path d="M2 7h10M8 3l4 4-4 4" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                    </button>
                  </div>
                </div>
              </div>

              <div class="comments-list">
                <p v-if="(comments[selectedArticle.id] ?? []).length === 0" class="comments-empty">
                  <svg width="32" height="32" viewBox="0 0 24 24" fill="none">
                    <path d="M21 12a8 8 0 0 1-11.5 7.2L4 21l1.8-5.5A8 8 0 1 1 21 12z"
                          stroke="currentColor" stroke-width="1.4" stroke-linejoin="round" />
                  </svg>
                  <span>Noch keine Kommentare. Sei der Erste!</span>
                </p>
                <div v-for="(c, idx) in (comments[selectedArticle.id] ?? [])" :key="idx"
                     class="comment" :class="{ 'comment-own': c.author === 'Du' }">
                  <div class="comment-avatar" :class="{ 'comment-avatar-own': c.author === 'Du' }">
                    {{ c.author[0] }}
                  </div>
                  <div class="comment-body">
                    <div class="comment-meta">
                      <strong>{{ c.author }}</strong>
                      <span v-if="c.author === 'Du'" class="comment-own-tag">Dein Kommentar</span>
                      <span class="comment-meta-sep">·</span>
                      <span>{{ c.date }}</span>
                      <button v-if="c.author === 'Du'"
                              class="comment-delete"
                              @click="deleteComment(selectedArticle.id, idx)"
                              aria-label="Kommentar löschen"
                              title="Kommentar löschen">
                        <svg width="13" height="13" viewBox="0 0 14 14" fill="none">
                          <path d="M2.5 4h9M5 4V2.5h4V4M3.5 4l.5 8h6l.5-8M6 6.5v3.5M8 6.5v3.5"
                                stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                      </button>
                    </div>
                    <p>{{ c.text }}</p>
                  </div>
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* ─── RESET ─────────────────────────────────────────── */
* { margin: 0; padding: 0; box-sizing: border-box; }
:global(html) { scroll-behavior: smooth; }

/* ─── DESIGN TOKENS ─────────────────────────────────── */
.app {
  --c-bg:             #fbfbfa;
  --c-surface:        #f4f4f2;
  --c-surface-2:      #eeeeec;
  --c-surface-hover:  #e8e8e6;
  --c-border:         #dcdcd6;
  --c-border-light:   #ececea;
  --c-text-primary:   #16201a;
  --c-text-secondary: #5d6660;
  --c-text-tertiary:  #8a918c;

  --c-accent:         #2d8a3e;
  --c-accent-2:       #34c759;
  --c-accent-deep:    #1f6e2f;
  --c-accent-light:   #e6f5ea;
  --c-accent-soft:    #f1faf3;

  --c-like:           #e85d75;
  --c-like-bg:        #fdecf0;

  --c-cat:            #3aaa35;
  --c-cat-text:       #ffffff;

  --radius-s:         10px;
  --radius-m:         14px;
  --radius-l:         20px;
  --radius-xl:        24px;

  --shadow-sm:        0 2px 4px rgba(20,30,25,.05), 0 4px 12px rgba(20,30,25,.04);
  --shadow-md:        0 4px 12px rgba(20,30,25,.08), 0 12px 32px rgba(20,30,25,.06);
  --shadow-lg:        0 12px 32px rgba(20,30,25,.10), 0 24px 64px rgba(20,30,25,.08);
  --shadow-modal:     0 24px 80px rgba(15,30,20,.22);

  --font-display:     'Fraunces', 'Cambria', Georgia, serif;
  --font-body:        'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  --ease:             cubic-bezier(.4, 0, .2, 1);
  --ease-out:         cubic-bezier(.16, 1, .3, 1);
  --ease-spring:      cubic-bezier(.34, 1.56, .64, 1);

  font-family: var(--font-body);
  color: var(--c-text-primary);
  background: var(--c-bg);
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  min-height: 100vh;
  position: relative;
  transition: background .35s var(--ease), color .35s var(--ease);
}

/* ─── DARK MODE ─────────────────────────────────────── */
.app.theme-dark {
  --c-bg:             #0d120e;
  --c-surface:        #161c18;
  --c-surface-2:      #1d241f;
  --c-surface-hover:  #242c26;
  --c-border:         #2a322c;
  --c-border-light:   #1d241f;
  --c-text-primary:   #f0f3ef;
  --c-text-secondary: #b6bdb8;
  --c-text-tertiary:  #7e857f;
  --c-accent:         #4cd169;
  --c-accent-2:       #6ddb87;
  --c-accent-deep:    #34c759;
  --c-accent-light:   #1a2a1f;
  --c-accent-soft:    #14201a;
  --c-like:           #ff7a91;
  --c-like-bg:        #2a1a1f;
  --shadow-modal:     0 24px 80px rgba(0,0,0,.6);
}

/* ─── HEADER ────────────────────────────────────────── */
.header {
  position: sticky;
  top: 0;
  z-index: 100;
  background: rgba(251, 251, 250, .82);
  backdrop-filter: saturate(180%) blur(20px);
  -webkit-backdrop-filter: saturate(180%) blur(20px);
  border-bottom: 1px solid var(--c-border-light);
  transition: background .35s var(--ease);
}
.theme-dark .header { background: rgba(13, 18, 14, .82); }

.header-inner {
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 28px;
  height: 72px;
  display: flex;
  align-items: center;
  gap: 28px;
}

.logo {
  display: flex;
  align-items: center;
  text-decoration: none;
  flex-shrink: 0;
  transition: transform .25s var(--ease-spring);
}
.logo:hover { transform: scale(1.03); }
.logo-img {
  height: 300px;
  width: auto;
  display: block;
  user-select: none;
}

.nav-links {
  display: flex;
  gap: 2px;
  flex: 1;
  justify-content: center;
}
.nav-link {
  font-size: 14px;
  font-weight: 500;
  color: var(--c-text-secondary);
  text-decoration: none;
  padding: 8px 14px;
  border-radius: var(--radius-s);
  transition: background .2s var(--ease), color .2s var(--ease);
  position: relative;
}
.nav-link::after {
  content: '';
  position: absolute;
  left: 14px; right: 14px; bottom: 4px;
  height: 2px;
  background: var(--c-accent);
  border-radius: 2px;
  transform: scaleX(0);
  transition: transform .25s var(--ease-out);
}
.nav-link:hover { color: var(--c-text-primary); background: var(--c-accent-soft); }
.nav-link:hover::after { transform: scaleX(1); }

.header-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}

.icon-btn {
  width: 36px; height: 36px;
  border: 1px solid var(--c-border);
  background: var(--c-bg);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--c-text-secondary);
  font-family: inherit;
  transition: all .2s var(--ease);
}
.icon-btn:hover {
  color: var(--c-accent);
  border-color: var(--c-accent);
  background: var(--c-accent-soft);
  transform: translateY(-1px);
}
.theme-toggle:hover { transform: rotate(15deg); }

.btn-login {
  font-size: 13px;
  font-weight: 600;
  color: #fff;
  background: linear-gradient(135deg, var(--c-accent-2) 0%, var(--c-accent) 100%);
  padding: 8px 18px;
  border-radius: 999px;
  text-decoration: none;
  transition: transform .2s var(--ease), box-shadow .2s var(--ease), filter .2s var(--ease);
  box-shadow: 0 2px 6px rgba(45,138,62,.25);
  margin-left: 4px;
}
.btn-login:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
  box-shadow: 0 6px 16px rgba(45,138,62,.32);
}

/* ─── LOADING ───────────────────────────────────────── */
.loading-state {
  text-align: center;
  padding: 140px 24px;
  color: var(--c-text-tertiary);
  font-size: 14px;
}
.spinner {
  width: 36px; height: 36px;
  margin: 0 auto 18px;
  border: 3px solid var(--c-border-light);
  border-top-color: var(--c-accent);
  border-radius: 50%;
  animation: spin .8s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg) } }

/* ─── MAIN ──────────────────────────────────────────── */
.main {
  max-width: 1280px;
  margin: 0 auto;
  padding: 36px 28px 100px;
  position: relative;
  animation: pageIn .5s var(--ease-out) both;
}
@keyframes pageIn {
  from { opacity: 0; transform: translateY(8px); }
  to   { opacity: 1; transform: none; }
}

.error-banner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fff8db;
  border: 1px solid #ecd06a;
  border-radius: var(--radius-m);
  padding: 14px 20px;
  margin-bottom: 28px;
  font-size: 13px;
  color: #6b5210;
}
.theme-dark .error-banner {
  background: #2a230a;
  border-color: #5a4818;
  color: #f0d878;
}
.btn-seed {
  font-size: 12px;
  font-weight: 600;
  padding: 7px 16px;
  border: 1px solid var(--c-border);
  border-radius: 999px;
  background: var(--c-bg);
  color: var(--c-text-primary);
  cursor: pointer;
  transition: background .2s var(--ease), transform .15s var(--ease);
  font-family: inherit;
}
.btn-seed:hover { background: var(--c-surface); transform: translateY(-1px); }
.btn-seed:disabled { opacity: .5; cursor: default; transform: none; }

/* ─── EYEBROW + HEADLINE ────────────────────────────── */
.eyebrow {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 12px;
  font-size: 12px;
  font-weight: 600;
  letter-spacing: .04em;
  text-transform: uppercase;
  color: var(--c-text-secondary);
}
.eyebrow-dot {
  width: 6px; height: 6px;
  border-radius: 50%;
  background: var(--c-accent-2);
  box-shadow: 0 0 0 4px rgba(52,199,89,.18);
  animation: pulse 2s var(--ease) infinite;
}
@keyframes pulse {
  0%, 100% { box-shadow: 0 0 0 4px rgba(52,199,89,.18); }
  50%      { box-shadow: 0 0 0 8px rgba(52,199,89,.06); }
}
.eyebrow-text { color: var(--c-text-tertiary); text-transform: none; letter-spacing: 0; font-weight: 500; }
.eyebrow-tag {
  margin-left: auto;
  padding: 4px 10px;
  background: var(--c-accent-soft);
  color: var(--c-accent);
  border-radius: 999px;
  font-size: 11px;
}

.page-headline {
  font-family: var(--font-display);
  font-size: clamp(34px, 5vw, 52px);
  font-weight: 600;
  letter-spacing: -1.5px;
  line-height: 1.05;
  color: var(--c-text-primary);
  margin-bottom: 32px;
  max-width: 820px;
}
.page-headline em {
  font-style: italic;
  font-weight: 500;
  color: var(--c-accent);
  font-variation-settings: "SOFT" 100, "WONK" 1;
}

/* ════════════════════════════════════════════════════ */
/* ─── MAGAZINE GRID ─────────────────────────────────── */
/* ════════════════════════════════════════════════════ */

.row-hero {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 18px;
  margin-bottom: 18px;
}
.row-trio {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
  margin-bottom: 56px;
}
.row-rest {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
}

@media (max-width: 980px) {
  .row-hero { grid-template-columns: 1fr; }
  .row-trio,
  .row-rest { grid-template-columns: 1fr 1fr; }
  .nav-links { display: none; }
}
@media (max-width: 640px) {
  .row-trio,
  .row-rest { grid-template-columns: 1fr; }
}

/* ─── MAG-CARD (gemeinsame Basis) ───────────────────── */
.mag-card {
  position: relative;
  display: block;
  border-radius: var(--radius-l);
  overflow: hidden;
  cursor: pointer;
  background: var(--c-surface);
  border: 1px solid var(--c-border-light);
  transition: transform .35s var(--ease-out), box-shadow .35s var(--ease-out);
  animation: cardIn .55s var(--ease-out) both;
  animation-delay: var(--delay, 0ms);
  isolation: isolate;
}
@keyframes cardIn {
  from { opacity: 0; transform: translateY(12px); }
  to   { opacity: 1; transform: none; }
}
.mag-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}
.mag-card:focus-visible {
  outline: 2px solid var(--c-accent);
  outline-offset: 3px;
}

.mag-card-big    { min-height: 480px; }
.mag-card-side   { min-height: 480px; }
.mag-card-medium { min-height: 360px; }
.mag-card-small  { min-height: 320px; }

@media (max-width: 980px) {
  .mag-card-big,
  .mag-card-side { min-height: 360px; }
}

.mag-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  z-index: 0;
  transition: transform 1s var(--ease-out);
}
.mag-card:hover .mag-img { transform: scale(1.05); }

.mag-shade {
  position: absolute;
  inset: 0;
  z-index: 1;
  background: linear-gradient(
    180deg,
    rgba(15,25,20,0)    0%,
    rgba(15,25,20,0)   30%,
    rgba(15,25,20,.45) 60%,
    rgba(15,25,20,.92) 100%
  );
  pointer-events: none;
}

.mag-body {
  position: absolute;
  left: 0; right: 0; bottom: 0;
  z-index: 2;
  padding: 24px 26px 22px;
  color: #fff;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.mag-card-big    .mag-body { padding: 32px 36px 28px; }
.mag-card-medium .mag-body,
.mag-card-small  .mag-body { padding: 20px 22px 18px; }

.mag-badges {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
  margin-bottom: 4px;
}
.pill {
  display: inline-flex;
  align-items: center;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: .03em;
  padding: 5px 12px;
  border-radius: 6px;
  white-space: nowrap;
}
.pill-cat {
  background: var(--c-cat);
  color: var(--c-cat-text);
}
.pill-outline {
  background: transparent;
  border: 1px solid var(--c-border);
  color: var(--c-text-secondary);
}

.mag-title {
  font-family: var(--font-display);
  font-weight: 600;
  letter-spacing: -.4px;
  line-height: 1.18;
  color: #fff;
  text-shadow: 0 2px 10px rgba(0,0,0,.35);
  display: -webkit-box;
  -webkit-line-clamp: 4;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.mag-title-big {
  font-size: clamp(24px, 2.6vw, 34px);
  -webkit-line-clamp: 3;
}
.mag-card-side .mag-title { font-size: 18px; -webkit-line-clamp: 4; }
.mag-card-medium .mag-title { font-size: 17px; -webkit-line-clamp: 4; }
.mag-card-small .mag-title { font-size: 16px; -webkit-line-clamp: 4; }

.mag-summary {
  font-size: 14px;
  line-height: 1.55;
  color: rgba(255,255,255,.88);
  font-family: var(--font-body);
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-top: 4px;
}
.mag-card-side .mag-summary,
.mag-card-medium .mag-summary,
.mag-card-small .mag-summary { display: none; }

.mag-foot {
  display: flex;
  align-items: center;
  gap: 14px;
  font-size: 12px;
  color: rgba(255,255,255,.78);
  margin-top: 6px;
}
.mag-date {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
.mag-source { font-weight: 500; }

/* ─── SECTION HEAD ──────────────────────────────────── */
.section-head {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  margin: 0 0 22px;
}
.section-title {
  font-family: var(--font-display);
  font-size: 26px;
  font-weight: 600;
  letter-spacing: -.5px;
  color: var(--c-text-primary);
}
.section-link {
  font-size: 13px;
  font-weight: 600;
  color: var(--c-accent);
  text-decoration: none;
  transition: opacity .2s var(--ease);
}
.section-link:hover { opacity: .7; }

/* ─── LOAD MORE ─────────────────────────────────────── */
.load-more-wrap {
  text-align: center;
  margin-top: 48px;
}
.btn-load-more {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
  color: var(--c-accent);
  border: 1.5px solid var(--c-accent);
  padding: 11px 28px;
  border-radius: 999px;
  text-decoration: none;
  transition: all .25s var(--ease);
  background: transparent;
}
.btn-load-more:hover {
  background: var(--c-accent);
  color: #fff;
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(45,138,62,.25);
}
.btn-load-more:hover svg { transform: translateX(3px); }
.btn-load-more svg { transition: transform .25s var(--ease-spring); }

/* ─── FOOTER ────────────────────────────────────────── */
.footer {
  border-top: 1px solid var(--c-border-light);
  background: var(--c-surface);
  margin-top: 40px;
}
.footer-inner {
  max-width: 1280px;
  margin: 0 auto;
  padding: 56px 28px 36px;
  display: grid;
  grid-template-columns: 1.6fr 1fr 1fr 1fr;
  gap: 44px;
}
.footer-brand .logo-img { height: 38px; }
@media (max-width: 760px) {
  .footer-inner { grid-template-columns: 1fr 1fr; gap: 28px; }
}
.footer-desc {
  font-size: 13.5px;
  color: var(--c-text-secondary);
  line-height: 1.6;
  margin-top: 14px;
  max-width: 320px;
}
.footer-col h6 {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: .08em;
  text-transform: uppercase;
  color: var(--c-text-secondary);
  margin-bottom: 14px;
}
.footer-col a {
  display: block;
  font-size: 13.5px;
  color: var(--c-text-tertiary);
  text-decoration: none;
  padding: 4px 0;
  transition: color .2s var(--ease), transform .15s var(--ease);
}
.footer-col a:hover { color: var(--c-accent); transform: translateX(2px); }
.footer-bottom {
  max-width: 1280px;
  margin: 0 auto;
  padding: 18px 28px;
  border-top: 1px solid var(--c-border-light);
  color: var(--c-text-tertiary);
  font-size: 12px;
}

/* ════════════════════════════════════════════════════ */
/* ─── MODAL ─────────────────────────────────────────── */
/* ════════════════════════════════════════════════════ */

.hp-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 25, 20, .55);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  z-index: 200;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  overflow-y: auto;
  padding: 48px 16px;
}
.theme-dark .hp-overlay { background: rgba(0,0,0,.7); }

.hp-dialog {
  position: relative;
  background: var(--c-bg);
  border-radius: var(--radius-xl);
  width: 100%;
  max-width: 720px;
  box-shadow: var(--shadow-modal);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - 96px);
  border: 1px solid var(--c-border-light);
}

.hp-dialog-head {
  position: sticky;
  top: 0;
  z-index: 5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 20px;
  border-bottom: 1px solid var(--c-border-light);
  background: var(--c-bg);
}
.hp-dialog-head-badges { display: flex; gap: 6px; }

.hp-dialog-close {
  width: 34px; height: 34px;
  border: none;
  background: var(--c-surface);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--c-text-secondary);
  transition: background .2s var(--ease), transform .2s var(--ease-spring), color .2s var(--ease);
}
.hp-dialog-close:hover {
  background: var(--c-text-primary);
  color: var(--c-bg);
  transform: rotate(90deg);
}

.hp-dialog-scroll { overflow-y: auto; flex: 1; }
.hp-dialog-media { position: relative; }
.hp-dialog-img {
  width: 100%;
  height: 280px;
  object-fit: cover;
  display: block;
}
.hp-dialog-content { padding: 32px 32px 8px; }
.hp-dialog-title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 600;
  letter-spacing: -.6px;
  line-height: 1.22;
  color: var(--c-text-primary);
  margin-bottom: 12px;
}
.hp-dialog-meta { margin-bottom: 20px; }
.hp-dialog-summary {
  font-size: 16px;
  line-height: 1.7;
  color: var(--c-text-secondary);
  margin-bottom: 24px;
}
.meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--c-text-tertiary);
  flex-wrap: wrap;
}
.sep {
  width: 3px; height: 3px;
  border-radius: 50%;
  background: currentColor;
  opacity: .6;
  flex-shrink: 0;
}

.btn-original {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 13.5px;
  font-weight: 600;
  color: var(--c-accent);
  border: 1.5px solid var(--c-accent);
  padding: 10px 22px;
  border-radius: 999px;
  text-decoration: none;
  transition: all .25s var(--ease);
  margin-bottom: 8px;
  background: transparent;
}
.btn-original:hover {
  background: var(--c-accent);
  color: #fff;
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(45,138,62,.25);
}
.btn-original:hover svg { transform: translate(2px,-2px); }
.btn-original svg { transition: transform .25s var(--ease-spring); }

/* ─── HP-SECTION (Likes & Kommentare) ───────────────── */
.hp-section {
  margin: 28px 32px 0;
  padding: 24px 26px 26px;
  border-radius: var(--radius-l);
  border: 1px solid var(--c-border-light);
  background: var(--c-bg);
}
.hp-section-like {
  background: radial-gradient(110% 100% at 0% 0%, var(--c-accent-soft), var(--c-bg) 70%);
  border-color: var(--c-accent-light);
}
.hp-section-comments {
  background: var(--c-surface);
  border-color: var(--c-border-light);
}

.hp-section-head {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  margin-bottom: 18px;
}
.hp-section-icon {
  width: 32px; height: 32px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.hp-section-icon-like { background: var(--c-like-bg); color: var(--c-like); }
.hp-section-icon-cmt  { background: var(--c-accent-light); color: var(--c-accent); }

.hp-section-headtext { flex: 1; min-width: 0; }
.hp-section-title {
  font-family: var(--font-display);
  font-size: 17px;
  font-weight: 600;
  color: var(--c-text-primary);
  letter-spacing: -.2px;
  margin-bottom: 2px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.hp-count-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 24px;
  height: 22px;
  padding: 0 8px;
  background: var(--c-accent);
  color: #fff;
  border-radius: 999px;
  font-family: var(--font-body);
  font-size: 12px;
  font-weight: 700;
}
.hp-section-sub {
  font-size: 13px;
  color: var(--c-text-tertiary);
  line-height: 1.5;
}

/* Like-Stat */
.like-stat {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 8px 4px;
}
.like-btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 12px 22px;
  border: 1.5px solid var(--c-border);
  background: var(--c-bg);
  border-radius: 999px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  color: var(--c-text-secondary);
  font-family: inherit;
  transition: all .2s var(--ease);
}
.like-btn:hover {
  border-color: var(--c-like);
  color: var(--c-like);
  transform: translateY(-1px);
}
.like-btn.active {
  background: var(--c-like);
  border-color: var(--c-like);
  color: #fff;
  box-shadow: 0 6px 18px rgba(232,93,117,.32);
}
.like-btn.active:hover { filter: brightness(1.05); color: #fff; }
.like-btn .like-icon { transition: transform .25s var(--ease-spring); }
.like-btn.active .like-icon { animation: heartbeat .55s var(--ease-spring); }
@keyframes heartbeat {
  0%   { transform: scale(1); }
  35%  { transform: scale(1.4); }
  60%  { transform: scale(.9); }
  100% { transform: scale(1); }
}

.like-count {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  text-align: right;
  line-height: 1;
}
.like-count-num {
  font-family: var(--font-display);
  font-size: 32px;
  font-weight: 700;
  color: var(--c-text-primary);
  letter-spacing: -1px;
}
.like-count-lbl {
  font-size: 11px;
  letter-spacing: .04em;
  text-transform: uppercase;
  color: var(--c-text-tertiary);
  margin-top: 4px;
  font-weight: 600;
}

/* Divider */
.hp-divider {
  display: flex;
  align-items: center;
  gap: 12px;
  margin: 28px 32px 0;
  padding: 0 4px;
}
.hp-divider-line { flex: 1; height: 1px; background: var(--c-border-light); }
.hp-divider-text {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: .12em;
  text-transform: uppercase;
  color: var(--c-text-tertiary);
}

/* Kommentar-Form */
.comment-form {
  display: flex;
  gap: 12px;
  margin-bottom: 22px;
}
.comment-form-avatar {
  width: 36px; height: 36px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--c-accent-2), var(--c-accent));
  color: #fff;
  font-weight: 700;
  font-size: 13px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.comment-form-input {
  flex: 1;
  background: var(--c-bg);
  border: 1.5px solid var(--c-border);
  border-radius: var(--radius-m);
  transition: border-color .2s var(--ease), box-shadow .2s var(--ease);
}
.comment-form-input:focus-within {
  border-color: var(--c-accent);
  box-shadow: 0 0 0 4px rgba(45,138,62,.10);
}
.comment-input {
  width: 100%;
  border: none;
  background: transparent;
  padding: 12px 14px 6px;
  font-size: 14px;
  font-family: inherit;
  color: var(--c-text-primary);
  resize: none;
  outline: none;
  line-height: 1.5;
}
.comment-form-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 14px 10px;
}
.comment-form-hint {
  font-size: 11px;
  color: var(--c-text-tertiary);
  font-variant-numeric: tabular-nums;
}
.btn-send {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
  color: #fff;
  background: linear-gradient(135deg, var(--c-accent-2), var(--c-accent));
  border: none;
  padding: 8px 18px;
  border-radius: 999px;
  cursor: pointer;
  font-family: inherit;
  transition: transform .15s var(--ease), filter .2s var(--ease), box-shadow .2s var(--ease);
  box-shadow: 0 2px 6px rgba(45,138,62,.25);
}
.btn-send:hover:not(:disabled) {
  transform: translateY(-1px);
  filter: brightness(1.05);
  box-shadow: 0 6px 14px rgba(45,138,62,.32);
}
.btn-send:hover:not(:disabled) svg { transform: translateX(3px); }
.btn-send svg { transition: transform .2s var(--ease-spring); }
.btn-send:disabled {
  opacity: .4;
  cursor: not-allowed;
  background: var(--c-text-tertiary);
  box-shadow: none;
}

/* Kommentar-Liste */
.comments-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.comments-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 10px;
  text-align: center;
  color: var(--c-text-tertiary);
  font-size: 13.5px;
  padding: 28px 16px 20px;
}
.comments-empty svg { opacity: .35; }

.comment {
  display: flex;
  gap: 10px;
  animation: cmtIn .35s var(--ease-out) both;
}
@keyframes cmtIn {
  from { opacity: 0; transform: translateY(6px); }
  to   { opacity: 1; transform: none; }
}

.comment-avatar {
  width: 36px; height: 36px;
  border-radius: 50%;
  background: var(--c-bg);
  border: 1.5px solid var(--c-border-light);
  color: var(--c-accent);
  font-weight: 700;
  font-size: 13px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.comment-avatar-own {
  background: linear-gradient(135deg, var(--c-accent-2), var(--c-accent));
  color: #fff;
  border-color: transparent;
}

.comment-body {
  background: var(--c-bg);
  border-radius: var(--radius-m);
  padding: 12px 16px;
  flex: 1;
  border: 1px solid var(--c-border-light);
  position: relative;
}
.comment-own .comment-body {
  border-color: var(--c-accent-light);
  background: var(--c-accent-soft);
}

.comment-meta {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 4px;
  font-size: 12px;
  color: var(--c-text-tertiary);
  flex-wrap: wrap;
}
.comment-meta strong { color: var(--c-text-primary); font-weight: 600; }
.comment-meta-sep { opacity: .5; }

.comment-own-tag {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: .04em;
  background: var(--c-accent);
  color: #fff;
  padding: 2px 7px;
  border-radius: 4px;
}

.comment-delete {
  margin-left: auto;
  width: 26px; height: 26px;
  border: none;
  background: transparent;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--c-text-tertiary);
  transition: all .15s var(--ease);
  font-family: inherit;
}
.comment-delete:hover {
  color: var(--c-like);
  background: var(--c-like-bg);
}

.comment-body p {
  margin: 0;
  font-size: 13.5px;
  color: var(--c-text-primary);
  line-height: 1.55;
}

.hp-section-comments { margin-bottom: 32px; }

/* ─── REDUCED MOTION ────────────────────────────────── */
@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: .01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: .01ms !important;
    scroll-behavior: auto !important;
  }
}
</style>

<style>
/* ─── GLOBAL: Schriften ─────────────────────────────── */
@import url('https://fonts.googleapis.com/css2?family=Fraunces:ital,opsz,wght@0,9..144,400..700;1,9..144,400..700&family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap');

/* ─── GLOBAL: Modal Transitions ─────────────────────── */
.hp-fade-enter-active { transition: opacity .25s ease; }
.hp-fade-leave-active { transition: opacity .2s ease; }
.hp-fade-enter-active .hp-dialog {
  transition: transform .35s cubic-bezier(.16,1,.3,1), opacity .25s ease;
}
.hp-fade-leave-active .hp-dialog {
  transition: transform .2s ease, opacity .2s ease;
}
.hp-fade-enter-from { opacity: 0; }
.hp-fade-enter-from .hp-dialog {
  opacity: 0;
  transform: translateY(28px) scale(.96);
}
.hp-fade-leave-to { opacity: 0; }
.hp-fade-leave-to .hp-dialog {
  opacity: 0;
  transform: translateY(12px) scale(.98);
}
</style>