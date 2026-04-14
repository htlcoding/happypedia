<script setup>
import { ref, onMounted, computed } from 'vue'
import {
  fetchArticles,
  fetchArticlesFromFeeds,
  likeArticle,
  dislikeArticle,
<<<<<<< HEAD
  deleteArticle
} from './services/api'
=======
} from './services/api'

// Backend-Artikel (description, imageUrl, upvotes/downvotes) in das
// vom Template erwartete Format bringen (summary, likes …).
function normalizeArticle(a) {
  return {
    ...a,
    summary: a.summary ?? a.description ?? '',
  }
}
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095

const articles = ref([])
const loading = ref(true)
const error = ref(null)
const selectedArticle = ref(null)
const commentInput = ref('')

const likes = ref({})
const liked = ref({})
const comments = ref({})
const imageErrors = ref({})

const visibleCount = ref(9)

const fallbackArticles = [
  {
    id: 1,
    title: 'Wiener Startup entwickelt App gegen Lebensmittelverschwendung',
    url: 'https://example.com/wiener-startup-food',
    source: 'derstandard.at',
    publishedAt: '2026-03-16T10:00:00Z',
    score: 92,
    summary: 'Ein junges Wiener Startup hat eine App entwickelt, die Supermärkte und Restaurants dabei hilft, überschüssige Lebensmittel an Bedürftige weiterzugeben – und damit Tonnen von Lebensmittelabfällen zu vermeiden.',
    imageUrl: null,
    upvotes: 0,
    downvotes: 0
  },
  {
    id: 2,
    title: 'Neue Studie: Ehrenamt stärkt mentale Gesundheit nachweislich',
    url: 'https://example.com/ehrenamt-studie',
    source: 'zeit.de',
    publishedAt: '2026-03-15T08:30:00Z',
    score: 88,
    summary: 'Eine großangelegte Studie mit über 10.000 Teilnehmern belegt: Wer regelmäßig ehrenamtlich tätig ist, berichtet signifikant seltener von Burnout, Einsamkeit und Depressionen.',
    imageUrl: null,
    upvotes: 0,
    downvotes: 0
  },
  {
    id: 3,
    title: 'Solarenergie-Rekord in Deutschland: 60% des Stroms aus Erneuerbaren',
    url: 'https://example.com/solar-rekord',
    source: 'tagesschau.de',
    publishedAt: '2026-03-16T14:00:00Z',
    score: 95,
    summary: 'Deutschland hat einen neuen Rekord bei erneuerbaren Energien aufgestellt: An einem sonnigen Frühlingstag deckten Solar-, Wind- und Wasserkraft gemeinsam 60 % des gesamten Strombedarfs.',
    imageUrl: null,
    upvotes: 0,
    downvotes: 0
  }
]

<<<<<<< HEAD
function normalizeArticle(article) {
  return {
    ...article,
    summary: article.summary ?? article.description ?? 'Kein Kurztext verfügbar.',
    imageUrl: typeof article.imageUrl === 'string' && article.imageUrl.trim() !== ''
      ? article.imageUrl.trim()
      : null,
    upvotes: article.upvotes ?? 0,
    downvotes: article.downvotes ?? 0
  }
}

function initLocalState(list) {
  list.forEach(a => {
    likes.value[a.id] = a.upvotes ?? 0
    liked.value[a.id] = liked.value[a.id] ?? false
    comments.value[a.id] = comments.value[a.id] ?? []
    imageErrors.value[a.id] = imageErrors.value[a.id] ?? false
  })
}

async function loadArticles() {
  try {
    const data = await fetchArticles()
    const normalized = (data ?? []).map(normalizeArticle)

    articles.value = normalized.length > 0 ? normalized : fallbackArticles
    error.value = normalized.length > 0
      ? null
      : 'Noch keine Artikel im Backend – Platzhalter-Daten werden angezeigt.'

    initLocalState(articles.value)
=======
async function loadArticles() {
  try {
    const data = await fetchArticles()
    const normalized = Array.isArray(data) ? data.map(normalizeArticle) : []
    articles.value = normalized.length > 0 ? normalized : fallbackArticles
    if (normalized.length > 0) error.value = null
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
  } catch (e) {
    console.warn('Backend nicht erreichbar:', e)
    error.value = 'Backend nicht erreichbar – Platzhalter-Daten werden angezeigt.'
    articles.value = fallbackArticles
<<<<<<< HEAD
    initLocalState(fallbackArticles)
  } finally {
    loading.value = false
=======
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
  }
}

<<<<<<< HEAD
onMounted(async () => {
  await loadArticles()
=======
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
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
})

const sortedArticles = computed(() => [...articles.value].sort((a, b) => b.score - a.score))
const featuredArticle = computed(() => sortedArticles.value[0] ?? null)
const mainArticles = computed(() => sortedArticles.value.slice(1, visibleCount.value + 1))
const sidebarArticles = computed(() => sortedArticles.value.slice(0, 5))
const hasMoreArticles = computed(() => sortedArticles.value.length > visibleCount.value + 1)

async function toggleLike(id) {
<<<<<<< HEAD
  const article = articles.value.find(a => a.id === id)
  if (!article) return

  try {
    let updated

    if (liked.value[id]) {
      updated = await dislikeArticle(id)
      liked.value[id] = false
    } else {
      updated = await likeArticle(id)
      liked.value[id] = true
    }

    const normalized = normalizeArticle(updated)
    const index = articles.value.findIndex(a => a.id === id)

    if (index !== -1) {
      articles.value[index] = normalized
    }

    if (selectedArticle.value?.id === id) {
      selectedArticle.value = normalized
    }

    likes.value[id] = normalized.upvotes ?? 0
  } catch (e) {
    alert('Like/Dislike fehlgeschlagen: ' + e.message)
=======
  // Optimistisches UI-Update
  const wasLiked = liked.value[id]
  if (wasLiked) { likes.value[id]--; liked.value[id] = false }
  else          { likes.value[id]++; liked.value[id] = true  }

  // Backend synchronisieren (nur für echte Artikel aus dem Backend)
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
    // Rollback
    if (wasLiked) { likes.value[id]++; liked.value[id] = true }
    else          { likes.value[id]--; liked.value[id] = false }
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
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

function placeholderImg(source, variant) {
  const sizes = { hero: '900x440', card: '540x300', thumb: '96x96', detail: '900x380' }
  const shades = { hero: 'f0f0f0/999', card: 'f5f5f5/aaa', thumb: 'eee/bbb', detail: 'f0f0f0/888' }
  return `https://placehold.co/${sizes[variant]}/${shades[variant]}?text=${encodeURIComponent(source)}`
}

function getImage(article, variant) {
  if (!article) return placeholderImg('HappyPedia', variant)
  if (imageErrors.value[article.id]) return placeholderImg(article.source ?? 'HappyPedia', variant)
  return article.imageUrl || placeholderImg(article.source ?? 'HappyPedia', variant)
}

function handleImageError(articleId) {
  imageErrors.value[articleId] = true
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
<<<<<<< HEAD
    await fetchArticlesFromFeeds()
    await loadArticles()
    alert('Artikel erfolgreich aus RSS-Feeds geladen.')
  } catch (e) {
    alert('Fetch fehlgeschlagen: ' + e.message)
=======
    const result = await fetchArticlesFromFeeds()
    const imported = result?.imported ?? 0
    alert(`${imported} Artikel aus den RSS-Feeds importiert.`)
    await loadArticles()
  } catch (e) {
    alert('Artikel-Import fehlgeschlagen: ' + e.message)
>>>>>>> 86a776a654b9b00679df1ee580e2b419e86cf095
  } finally {
    seeding.value = false
  }
}

async function handleDeleteArticle(id) {
  try {
    await deleteArticle(id)
    articles.value = articles.value.filter(a => a.id !== id)

    delete likes.value[id]
    delete liked.value[id]
    delete comments.value[id]
    delete imageErrors.value[id]

    if (selectedArticle.value?.id === id) {
      closeArticle()
    }
  } catch (e) {
    alert('Löschen fehlgeschlagen: ' + e.message)
  }
}

function loadMoreArticles() {
  visibleCount.value += 6
}
</script>

<template>
  <div class="app">

    <!-- HEADER -->
    <header class="header">
      <div class="header-inner">
        <a href="#" class="logo">
          <span class="logo-mark">H</span>
          <span class="logo-text">HappyPedia</span>
        </a>
        <nav class="nav-links">
          <a v-for="cat in categories" :key="cat.label" href="#" class="nav-link">{{ cat.label }}</a>
        </nav>
        <div class="header-actions">
          <input type="text" class="search-field" placeholder="Suchen …" />
          <a href="#" class="btn-login">Anmelden</a>
        </div>
      </div>
    </header>

    <!-- LOADING -->
    <div v-if="loading" class="loading-state">
      <div class="spinner" />
      <p>Artikel werden geladen …</p>
    </div>

    <!-- CONTENT -->
    <main v-else class="main">

      <!-- Error banner -->
      <div v-if="error" class="error-banner">
        <span>{{ error }}</span>
        <button class="btn-seed" :disabled="seeding" @click="handleSeed">
          {{ seeding ? 'Importiere …' : 'Artikel aus RSS laden' }}
        </button>
      </div>

      <!-- Date line -->
      <p class="dateline">{{ formatDateLong() }}</p>

      <div class="grid-layout">

        <!-- PRIMARY COLUMN -->
        <div class="col-primary">

          <!-- FEATURED -->
          <article v-if="featuredArticle" class="featured" @click="openArticle(featuredArticle)" role="button"
            tabindex="0" @keydown.enter="openArticle(featuredArticle)">
            <img :src="getImage(featuredArticle, 'hero')" :alt="featuredArticle.title" class="featured-img"
              @error="handleImageError(featuredArticle.id)" />
            <div class="featured-body">
              <span class="badge">{{ scoreBadge(featuredArticle.score) }}</span>
              <h1 class="featured-title">{{ featuredArticle.title }}</h1>
              <p class="featured-summary">{{ featuredArticle.summary }}</p>
              <div class="meta">
                <span>{{ featuredArticle.source }}</span>
                <span class="sep" />
                <span>{{ formatDate(featuredArticle.publishedAt) }}</span>
                <span class="sep" />
                <span>Score {{ featuredArticle.score }}</span>
              </div>
            </div>
          </article>

          <!-- SECTION -->
          <h2 class="section-title">Aktuelle Happy News</h2>

          <!-- CARDS -->
          <div class="card-grid">
            <article v-for="art in mainArticles" :key="art.id" class="card" @click="openArticle(art)" role="button"
              tabindex="0" @keydown.enter="openArticle(art)"><img :src="getImage(art, 'card')" :alt="art.title"
                class="card-img" @error="handleImageError(art.id)" />
              <div class="card-body">
                <div class="card-badges">
                  <span class="badge">{{ scoreBadge(art.score) }}</span>
                  <span class="badge badge-outline">{{ art.source }}</span>
                </div>
                <h3 class="card-title">{{ art.title }}</h3>
                <div class="meta">
                  <span>{{ formatDate(art.publishedAt) }}</span>
                  <span class="sep" />
                  <span>Score {{ art.score }}</span>
                </div>
              </div>
            </article>
          </div>

          <div class="load-more-wrap">
            <button v-if="hasMoreArticles" class="btn-load-more" @click="loadMoreArticles">
              Weitere Artikel
            </button>
          </div>
        </div>

        <!-- SIDEBAR -->
        <aside class="col-sidebar">

          <!-- Kategorien -->
          <div class="widget">
            <h4 class="widget-title">Kategorien</h4>
            <div class="tag-cloud">
              <a v-for="cat in categories" :key="cat.label" href="#" class="tag">{{ cat.label }}</a>
            </div>
          </div>

          <!-- Top Scored -->
          <div class="widget">
            <h4 class="widget-title">Höchster Score</h4>
            <ol class="rank-list">
              <li v-for="(art, i) in sidebarArticles" :key="art.id" class="rank-item" @click="openArticle(art)"
                role="button" tabindex="0" @keydown.enter="openArticle(art)">
                <span class="rank-num">{{ i + 1 }}</span>
                <div class="rank-info">
                  <p class="rank-title">{{ art.title }}</p>
                  <span class="rank-meta">{{ art.source }} · Score {{ art.score }}</span>
                </div>
              </li>
            </ol>
          </div>
        </aside>
      </div>
    </main>

    <!-- FOOTER -->
    <footer class="footer">
      <div class="footer-inner">
        <div class="footer-brand">
          <a href="#" class="logo">
            <span class="logo-mark">H</span>
            <span class="logo-text" style="color: var(--c-text-tertiary)">HappyPedia</span>
          </a>
          <p class="footer-desc">Positive Nachrichten aus dem DACH-Raum und der Welt.</p>
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
        <small>© 2026 HappyPedia</small>
      </div>
    </footer>

    <!-- DETAIL MODAL -->
    <Transition name="hp-fade">
      <div v-if="selectedArticle" class="hp-overlay" @click.self="closeArticle">
        <div class="hp-dialog" role="dialog" aria-modal="true" @click.stop>

          <div class="hp-dialog-head">
            <div class="hp-dialog-head-badges">
              <span class="badge">{{ scoreBadge(selectedArticle.score) }}</span>
              <span class="badge badge-outline">{{ selectedArticle.source }}</span>
            </div>
            <button class="hp-dialog-close" @click="closeArticle" aria-label="Schließen">
              <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
                <path d="M4 4l10 10M14 4L4 14" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              </svg>
            </button>
          </div>

          <div class="hp-dialog-scroll">
            <img :src="getImage(selectedArticle, 'detail')" :alt="selectedArticle.title" class="hp-dialog-img"
              @error="handleImageError(selectedArticle.id)" />

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
                Originalartikel öffnen
                <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                  <path d="M5 2h7v7M12 2L2 12" stroke="currentColor" stroke-width="1.4" stroke-linecap="round"
                    stroke-linejoin="round" />
                </svg>
              </a>

              <!-- Like -->
              <div class="like-row">
                <button class="like-btn" :class="{ active: liked[selectedArticle.id] }"
                  @click="toggleLike(selectedArticle.id)">
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path
                      d="M7 11v10M3 13v6a2 2 0 002 2h11.6a2 2 0 002-1.6l1.3-8A2 2 0 0017.9 9H14V5a3 3 0 00-3-3l-1 1-3 6v2"
                      :fill="liked[selectedArticle.id] ? 'currentColor' : 'none'" stroke="currentColor"
                      stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
                  </svg>
                  <span>{{ likes[selectedArticle.id] ?? 0 }}</span>
                </button>
                <span class="like-hint">{{ liked[selectedArticle.id] ? 'Danke für dein Like!' : 'Hilf anderen, gute Nachrichten zu finden.' }}</span>
              </div>

              <!-- Kommentare -->
              <div class="comments">
                <h4 class="comments-title">
                  Kommentare
                  <span class="comments-count">{{ (comments[selectedArticle.id] ?? []).length }}</span>
                </h4>

                <div class="comments-list">
                  <p v-if="(comments[selectedArticle.id] ?? []).length === 0" class="comments-empty">
                    Noch keine Kommentare – schreib den ersten.
                  </p>
                  <div v-for="(c, idx) in (comments[selectedArticle.id] ?? [])" :key="idx" class="comment">
                    <div class="comment-avatar">{{ c.author[0] }}</div>
                    <div class="comment-body">
                      <div class="comment-meta">
                        <strong>{{ c.author }}</strong>
                        <span>{{ c.date }}</span>
                      </div>
                      <p>{{ c.text }}</p>
                    </div>
                  </div>
                </div>

                <div class="comment-form">
                  <textarea v-model="commentInput" class="comment-input" placeholder="Dein Kommentar …" rows="2"
                    @keydown.ctrl.enter="submitComment(selectedArticle.id)"
                    @keydown.meta.enter="submitComment(selectedArticle.id)"></textarea>
                  <button class="btn-send" :disabled="!commentInput.trim()" @click="submitComment(selectedArticle.id)">
                    Senden
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* ─── RESET ──────────────────────────────────────────── */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.app {
  --c-bg: #ffffff;
  --c-surface: #f5f5f7;
  --c-surface-hover: #ececee;
  --c-border: #d2d2d7;
  --c-border-light: #e8e8ed;
  --c-text-primary: #1d1d1f;
  --c-text-secondary: #6e6e73;
  --c-text-tertiary: #86868b;
  --c-accent: #2d8a3e;
  --c-accent-light: #e8f5e9;
  --c-accent-hover: #24712f;
  --c-dark: #1d1d1f;
  --radius-s: 8px;
  --radius-m: 12px;
  --radius-l: 20px;
  --shadow-card: 0 1px 3px rgba(0, 0, 0, .06), 0 4px 12px rgba(0, 0, 0, .04);
  --shadow-modal: 0 24px 80px rgba(0, 0, 0, .18);
  --font-display: 'SF Pro Display', -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, sans-serif;
  --font-body: 'SF Pro Text', -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, sans-serif;
  --transition: .2s cubic-bezier(.4, 0, .2, 1);

  font-family: var(--font-body);
  color: var(--c-text-primary);
  background: var(--c-bg);
  -webkit-font-smoothing: antialiased;
  min-height: 100vh;
}

/* ─── HEADER ─────────────────────────────────────────── */
.header {
  position: sticky;
  top: 0;
  z-index: 100;
  background: rgba(255, 255, 255, .72);
  backdrop-filter: saturate(180%) blur(20px);
  -webkit-backdrop-filter: saturate(180%) blur(20px);
  border-bottom: 1px solid var(--c-border-light);
}

.header-inner {
  max-width: 1120px;
  margin: 0 auto;
  padding: 0 24px;
  height: 52px;
  display: flex;
  align-items: center;
  gap: 32px;
}

.logo {
  display: flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  flex-shrink: 0;
}

.logo-mark {
  width: 28px;
  height: 28px;
  background: var(--c-accent);
  color: #fff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: var(--font-display);
  font-weight: 700;
  font-size: 15px;
}

.logo-text {
  font-family: var(--font-display);
  font-weight: 600;
  font-size: 18px;
  color: var(--c-text-primary);
  letter-spacing: -0.3px;
}

.nav-links {
  display: flex;
  gap: 4px;
  flex: 1;
}

.nav-link {
  font-size: 13px;
  color: var(--c-text-secondary);
  text-decoration: none;
  padding: 6px 12px;
  border-radius: var(--radius-s);
  transition: background var(--transition), color var(--transition);
}

.nav-link:hover {
  background: var(--c-surface);
  color: var(--c-text-primary);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.search-field {
  width: 180px;
  height: 32px;
  border: 1px solid var(--c-border);
  border-radius: var(--radius-s);
  padding: 0 12px;
  font-size: 13px;
  background: var(--c-surface);
  outline: none;
  transition: border-color var(--transition), box-shadow var(--transition);
}

.search-field:focus {
  border-color: var(--c-accent);
  box-shadow: 0 0 0 3px rgba(45, 138, 62, .12);
}

.btn-login {
  font-size: 13px;
  font-weight: 500;
  color: #fff;
  background: var(--c-accent);
  padding: 6px 16px;
  border-radius: var(--radius-s);
  text-decoration: none;
  transition: background var(--transition);
}

.btn-login:hover {
  background: var(--c-accent-hover);
}

/* ─── LOADING ────────────────────────────────────────── */
.loading-state {
  text-align: center;
  padding: 120px 24px;
  color: var(--c-text-tertiary);
  font-size: 14px;
}

.spinner {
  width: 32px;
  height: 32px;
  margin: 0 auto 16px;
  border: 3px solid var(--c-border-light);
  border-top-color: var(--c-accent);
  border-radius: 50%;
  animation: spin .7s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg)
  }
}

/* ─── MAIN ───────────────────────────────────────────── */
.main {
  max-width: 1120px;
  margin: 0 auto;
  padding: 32px 24px 80px;
}

.error-banner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fffbe6;
  border: 1px solid #f5d442;
  border-radius: var(--radius-m);
  padding: 12px 20px;
  margin-bottom: 24px;
  font-size: 13px;
}

.btn-seed {
  font-size: 12px;
  font-weight: 500;
  padding: 5px 14px;
  border: 1px solid var(--c-border);
  border-radius: var(--radius-s);
  background: #fff;
  cursor: pointer;
  transition: background var(--transition);
}

.btn-seed:hover {
  background: var(--c-surface);
}

.btn-seed:disabled {
  opacity: .5;
  cursor: default;
}

.dateline {
  font-size: 13px;
  color: var(--c-text-tertiary);
  margin-bottom: 28px;
  letter-spacing: .01em;
}

/* ─── GRID LAYOUT ────────────────────────────────────── */
.grid-layout {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 48px;
}

@media (max-width: 960px) {
  .grid-layout {
    grid-template-columns: 1fr;
    gap: 40px;
  }

  .nav-links {
    display: none;
  }

  .search-field {
    width: 140px;
  }
}

/* ─── FEATURED ───────────────────────────────────────── */
.featured {
  border-radius: var(--radius-l);
  overflow: hidden;
  background: var(--c-surface);
  cursor: pointer;
  transition: transform var(--transition), box-shadow var(--transition);
  margin-bottom: 40px;
}

.featured:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-card);
}

.featured-img {
  width: 100%;
  height: 360px;
  object-fit: cover;
  display: block;
}

.featured-body {
  padding: 28px 32px 32px;
}

.featured-title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 700;
  letter-spacing: -0.5px;
  line-height: 1.2;
  margin: 12px 0 10px;
  color: var(--c-text-primary);
}

.featured-summary {
  font-size: 15px;
  line-height: 1.6;
  color: var(--c-text-secondary);
  margin-bottom: 16px;
}

/* ─── BADGE ──────────────────────────────────────────── */
.badge {
  display: inline-block;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: .03em;
  text-transform: uppercase;
  padding: 3px 10px;
  border-radius: 6px;
  background: var(--c-accent-light);
  color: var(--c-accent);
}

.badge-outline {
  background: transparent;
  border: 1px solid var(--c-border);
  color: var(--c-text-secondary);
}

/* ─── META ───────────────────────────────────────────── */
.meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--c-text-tertiary);
}

.sep {
  width: 3px;
  height: 3px;
  border-radius: 50%;
  background: var(--c-text-tertiary);
  flex-shrink: 0;
}

/* ─── SECTION TITLE ──────────────────────────────────── */
.section-title {
  font-family: var(--font-display);
  font-size: 22px;
  font-weight: 700;
  letter-spacing: -0.3px;
  margin-bottom: 20px;
}

/* ─── CARDS ──────────────────────────────────────────── */
.card-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

@media (max-width: 720px) {
  .card-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 480px) {
  .card-grid {
    grid-template-columns: 1fr;
  }
}

.card {
  border-radius: var(--radius-m);
  overflow: hidden;
  background: var(--c-bg);
  border: 1px solid var(--c-border-light);
  cursor: pointer;
  transition: transform var(--transition), box-shadow var(--transition);
}

.card:hover {
  transform: translateY(-3px);
  box-shadow: var(--shadow-card);
}

.card-img {
  width: 100%;
  height: 160px;
  object-fit: cover;
  display: block;
}

.card-body {
  padding: 16px 18px 20px;
}

.card-badges {
  display: flex;
  gap: 6px;
  margin-bottom: 10px;
}

.card-title {
  font-family: var(--font-display);
  font-size: 15px;
  font-weight: 600;
  line-height: 1.35;
  letter-spacing: -0.1px;
  color: var(--c-text-primary);
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.load-more-wrap {
  text-align: center;
  margin-top: 36px;
}

.btn-load-more {
  font-size: 14px;
  font-weight: 500;
  color: var(--c-accent);
  border: 1px solid var(--c-accent);
  padding: 10px 36px;
  border-radius: var(--radius-s);
  text-decoration: none;
  transition: background var(--transition), color var(--transition);
}

.btn-load-more:hover {
  background: var(--c-accent);
  color: #fff;
}

/* ─── SIDEBAR ────────────────────────────────────────── */
.widget {
  padding: 24px;
  background: var(--c-bg);
  border: 1px solid var(--c-border-light);
  border-radius: var(--radius-m);
  margin-bottom: 20px;
}

.widget-title {
  font-family: var(--font-display);
  font-size: 15px;
  font-weight: 600;
  margin-bottom: 12px;
  letter-spacing: -0.1px;
}

.tag-cloud {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.tag {
  font-size: 12px;
  font-weight: 500;
  padding: 5px 14px;
  border: 1px solid var(--c-border);
  border-radius: 999px;
  text-decoration: none;
  color: var(--c-text-secondary);
  transition: background var(--transition), color var(--transition);
}

.tag:hover {
  background: var(--c-accent-light);
  color: var(--c-accent);
  border-color: var(--c-accent-light);
}

/* ─── RANK LIST ──────────────────────────────────────── */
.rank-list {
  list-style: none;
}

.rank-item {
  display: flex;
  align-items: flex-start;
  gap: 14px;
  padding: 14px 0;
  border-bottom: 1px solid var(--c-border-light);
  cursor: pointer;
  transition: background var(--transition);
}

.rank-item:last-child {
  border-bottom: none;
}

.rank-item:hover {
  background: var(--c-surface);
  margin: 0 -12px;
  padding-left: 12px;
  padding-right: 12px;
  border-radius: var(--radius-s);
}

.rank-num {
  font-family: var(--font-display);
  font-size: 20px;
  font-weight: 700;
  color: var(--c-border);
  min-width: 24px;
  line-height: 1;
  padding-top: 2px;
}

.rank-info {
  flex: 1;
}

.rank-title {
  font-size: 13px;
  font-weight: 600;
  line-height: 1.4;
  color: var(--c-text-primary);
  margin-bottom: 4px;
}

.rank-meta {
  font-size: 11px;
  color: var(--c-text-tertiary);
}

/* ─── FOOTER ─────────────────────────────────────────── */
.footer {
  border-top: 1px solid var(--c-border-light);
  background: var(--c-surface);
}

.footer-inner {
  max-width: 1120px;
  margin: 0 auto;
  padding: 48px 24px 32px;
  display: grid;
  grid-template-columns: 1.5fr 1fr 1fr 1fr;
  gap: 40px;
}

@media (max-width: 720px) {
  .footer-inner {
    grid-template-columns: 1fr 1fr;
    gap: 24px;
  }
}

.footer-desc {
  font-size: 13px;
  color: var(--c-text-tertiary);
  line-height: 1.5;
  margin-top: 8px;
}

.footer-col h6 {
  font-size: 12px;
  font-weight: 600;
  letter-spacing: .05em;
  text-transform: uppercase;
  color: var(--c-text-secondary);
  margin-bottom: 14px;
}

.footer-col a {
  display: block;
  font-size: 13px;
  color: var(--c-text-tertiary);
  text-decoration: none;
  padding: 3px 0;
  transition: color var(--transition);
}

.footer-col a:hover {
  color: var(--c-text-primary);
}

.footer-bottom {
  max-width: 1120px;
  margin: 0 auto;
  padding: 16px 24px;
  border-top: 1px solid var(--c-border-light);
  color: var(--c-text-tertiary);
  font-size: 12px;
}

/* ─── MODAL ──────────────────────────────────────────── */

.hp-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, .45);
  z-index: 200;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  overflow-y: auto;
  padding: 48px 16px;
}

.hp-dialog {
  background: #fff;
  border-radius: var(--radius-l);
  width: 100%;
  max-width: 680px;
  box-shadow: var(--shadow-modal);
  overflow: hidden;
}

.hp-dialog-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 20px;
  border-bottom: 1px solid var(--c-border-light);
}

.hp-dialog-head-badges {
  display: flex;
  gap: 6px;
}

.hp-dialog-close {
  width: 32px;
  height: 32px;
  border: none;
  background: var(--c-surface);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--c-text-secondary);
  transition: background var(--transition);
}

.hp-dialog-close:hover {
  background: var(--c-surface-hover);
}

.hp-dialog-scroll {
  max-height: calc(100vh - 10rem);
  overflow-y: auto;
}

.hp-dialog-img {
  width: 100%;
  height: 220px;
  object-fit: cover;
  display: block;
}

.hp-dialog-content {
  padding: 28px;
}

.hp-dialog-title {
  font-family: var(--font-display);
  font-size: 24px;
  font-weight: 700;
  letter-spacing: -0.4px;
  line-height: 1.25;
  color: var(--c-text-primary);
  margin-bottom: 8px;
}

.hp-dialog-meta {
  margin-bottom: 16px;
}

.hp-dialog-summary {
  font-size: 15px;
  line-height: 1.7;
  color: var(--c-text-secondary);
  margin-bottom: 20px;
}

.btn-original {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 500;
  color: var(--c-accent);
  border: 1px solid var(--c-accent);
  padding: 8px 20px;
  border-radius: var(--radius-s);
  text-decoration: none;
  transition: background var(--transition), color var(--transition);
  margin-bottom: 28px;
}

.btn-original:hover {
  background: var(--c-accent);
  color: #fff;
}

/* ─── LIKE ROW ───────────────────────────────────────── */
.like-row {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 16px 20px;
  background: var(--c-surface);
  border-radius: var(--radius-m);
  margin-bottom: 28px;
}

.like-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 18px;
  border: 1.5px solid var(--c-border);
  background: #fff;
  border-radius: 999px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  color: var(--c-text-secondary);
  transition: all var(--transition);
}

.like-btn:hover {
  border-color: var(--c-accent);
  color: var(--c-accent);
}

.like-btn.active {
  background: var(--c-accent);
  border-color: var(--c-accent);
  color: #fff;
}

.like-btn.active:hover {
  background: var(--c-accent-hover);
  border-color: var(--c-accent-hover);
  color: #fff;
}

.like-hint {
  font-size: 12px;
  color: var(--c-text-tertiary);
}

/* ─── COMMENTS ───────────────────────────────────────── */
.comments {
  background: var(--c-surface);
  border-radius: var(--radius-l);
  padding: 24px;
  margin-top: 8px;
}

.comments-title {
  font-family: var(--font-display);
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 16px;
}

.comments-count {
  font-weight: 400;
  color: var(--c-text-tertiary);
  font-size: 14px;
}

.comments-empty {
  text-align: center;
  color: var(--c-text-tertiary);
  font-size: 13px;
  padding: 20px;
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 20px;
}

.comment {
  display: flex;
  gap: 10px;
}

.comment-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #fff;
  color: var(--c-text-secondary);
  font-weight: 600;
  font-size: 13px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.comment-body {
  background: #fff;
  border-radius: var(--radius-m);
  padding: 10px 14px;
  flex: 1;
}

.comment-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
  font-size: 12px;
  color: var(--c-text-tertiary);
}

.comment-meta strong {
  color: var(--c-text-primary);
}

.comment-body p {
  margin: 0;
  font-size: 13px;
  color: var(--c-text-primary);
  line-height: 1.5;
}

.comment-form {
  border-top: 1px solid var(--c-border-light);
  padding-top: 16px;
  display: flex;
  gap: 10px;
  align-items: flex-end;
}

.comment-input {
  flex: 1;
  border: 1.5px solid var(--c-border);
  border-radius: var(--radius-m);
  padding: 10px 14px;
  font-size: 13px;
  resize: none;
  outline: none;
  font-family: inherit;
  color: var(--c-text-primary);
  background: #fff;
  transition: border-color var(--transition), box-shadow var(--transition);
}

.comment-input:focus {
  border-color: var(--c-accent);
  box-shadow: 0 0 0 3px rgba(45, 138, 62, .1);
}

.btn-send {
  font-size: 13px;
  font-weight: 600;
  color: #fff;
  background: var(--c-accent);
  border: none;
  padding: 10px 24px;
  border-radius: var(--radius-m);
  cursor: pointer;
  transition: background var(--transition), transform var(--transition);
  white-space: nowrap;
}

.btn-send:hover {
  background: var(--c-accent-hover);
  transform: scale(1.02);
}

.btn-send:disabled {
  opacity: .35;
  cursor: default;
  transform: none;
}
</style>

<style>
.hp-fade-enter-active {
  transition: opacity .25s ease;
}

.hp-fade-leave-active {
  transition: opacity .2s ease;
}

.hp-fade-enter-active .hp-dialog {
  transition: transform .3s cubic-bezier(.16, 1, .3, 1), opacity .25s ease;
}

.hp-fade-leave-active .hp-dialog {
  transition: transform .2s ease, opacity .2s ease;
}

.hp-fade-enter-from {
  opacity: 0;
}

.hp-fade-enter-from .hp-dialog {
  opacity: 0;
  transform: translateY(24px) scale(.97);
}

.hp-fade-leave-to {
  opacity: 0;
}

.hp-fade-leave-to .hp-dialog {
  opacity: 0;
  transform: translateY(12px) scale(.98);
}
</style>