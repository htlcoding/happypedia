<script setup>
import { ref, onMounted, computed } from 'vue'
import { fetchArticles, seedArticles } from './services/api'

// ── State ──────────────────────────────────────────────
const articles        = ref([])
const loading         = ref(true)
const error           = ref(null)
const selectedArticle = ref(null)   // für Detail-Ansicht
const commentInput    = ref('')

// Likes & Kommentare (lokal, bis Backend da ist)
const likes    = ref({})   // { articleId: count }
const liked    = ref({})   // { articleId: bool }
const comments = ref({})   // { articleId: [{ author, text, date }] }

// ── Fallback-Daten ────────────────────────────────────
const fallbackArticles = [
  {
    id: 1,
    title: 'Wiener Startup entwickelt App gegen Lebensmittelverschwendung',
    url: 'https://example.com/wiener-startup-food',
    source: 'derstandard.at',
    publishedAt: '2026-03-16T10:00:00Z',
    score: 92,
    summary: 'Ein junges Wiener Startup hat eine App entwickelt, die Supermärkte und Restaurants dabei hilft, überschüssige Lebensmittel an Bedürftige weiterzugeben – und damit Tonnen von Lebensmittelabfällen zu vermeiden.',
  },
  {
    id: 2,
    title: 'Neue Studie: Ehrenamt stärkt mentale Gesundheit nachweislich',
    url: 'https://example.com/ehrenamt-studie',
    source: 'zeit.de',
    publishedAt: '2026-03-15T08:30:00Z',
    score: 88,
    summary: 'Eine großangelegte Studie mit über 10.000 Teilnehmern belegt: Wer regelmäßig ehrenamtlich tätig ist, berichtet signifikant seltener von Burnout, Einsamkeit und Depressionen.',
  },
  {
    id: 3,
    title: 'Solarenergie-Rekord in Deutschland: 60% des Stroms aus Erneuerbaren',
    url: 'https://example.com/solar-rekord',
    source: 'tagesschau.de',
    publishedAt: '2026-03-16T14:00:00Z',
    score: 95,
    summary: 'Deutschland hat einen neuen Rekord bei erneuerbaren Energien aufgestellt: An einem sonnigen Frühlingstag deckten Solar-, Wind- und Wasserkraft gemeinsam 60 % des gesamten Strombedarfs.',
  },
  {
    id: 4,
    title: 'Schweizer Forscher finden neuen Ansatz gegen Antibiotikaresistenz',
    url: 'https://example.com/antibiotika-forschung',
    source: 'nzz.ch',
    publishedAt: '2026-03-14T12:00:00Z',
    score: 85,
    summary: 'Forschende der ETH Zürich haben einen molekularen Mechanismus entdeckt, der resistente Bakterien wieder für bestehende Antibiotika empfänglich machen könnte – ein Durchbruch für die Medizin.',
  },
  {
    id: 5,
    title: 'Community Gardens in Graz: Nachbarschaft wächst zusammen',
    url: 'https://example.com/community-gardens-graz',
    source: 'kleinezeitung.at',
    publishedAt: '2026-03-17T06:00:00Z',
    score: 78,
    summary: 'In Graz entstehen immer mehr Gemeinschaftsgärten, in denen Menschen verschiedener Herkunft gemeinsam anbauen, kochen und feiern. Ein Grazer Verein koordiniert mittlerweile 24 solcher Projekte.',
  },
  {
    id: 6,
    title: 'Berliner Schüler gewinnen internationalen Robotik-Wettbewerb',
    url: 'https://example.com/robotik-berlin',
    source: 'spiegel.de',
    publishedAt: '2026-03-15T16:00:00Z',
    score: 82,
    summary: 'Ein Team der Geschwister-Scholl-Oberschule Berlin hat beim World Robot Olympiad den ersten Platz in der Kategorie "Future Engineers" belegt – und damit alle 47 Teilnehmerländer hinter sich gelassen.',
  },
  {
    id: 7,
    title: 'Neues Bildungsprogramm macht Coding für alle Altersgruppen zugänglich',
    url: 'https://example.com/coding-bildung',
    source: 'golem.de',
    publishedAt: '2026-03-13T09:00:00Z',
    score: 74,
    summary: 'Das neue Förderprogramm "CodeForAll" bietet kostenlosen Programmierunterricht für Kinder ab 6 Jahren bis hin zu Senioren – und verzeichnet bereits über 50.000 aktive Teilnehmer in Deutschland.',
  },
  {
    id: 8,
    title: 'Österreich investiert Milliarden in Bahnausbau für klimafreundliches Reisen',
    url: 'https://example.com/bahn-ausbau-at',
    source: 'orf.at',
    publishedAt: '2026-03-17T12:00:00Z',
    score: 90,
    summary: 'Das Klimaschutzministerium hat ein 18-Milliarden-Euro-Paket für den Ausbau des österreichischen Schienennetzes beschlossen – mit neuen Hochgeschwindigkeitstrassen und dichterem Regionalverkehr.',
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

  // Lokale Like/Comment-Initialisierung
  const initId = (list) => list.forEach(a => {
    likes.value[a.id]    = likes.value[a.id]    ?? Math.floor(Math.random() * 40 + 5)
    liked.value[a.id]    = liked.value[a.id]    ?? false
    comments.value[a.id] = comments.value[a.id] ?? []
  })
  initId(fallbackArticles)
})

// ── Computed: Nach Score sortieren, dann aufteilen ───
const sortedArticles = computed(() =>
  [...articles.value].sort((a, b) => b.score - a.score)
)

const featuredArticle = computed(() => sortedArticles.value[0] ?? null)
const mainArticles    = computed(() => sortedArticles.value.slice(1, 4))
const sidebarArticles = computed(() => sortedArticles.value.slice(0, 5))   // Top 5 im Sidebar

// ── Like-Logik ────────────────────────────────────────
function toggleLike(id) {
  if (liked.value[id]) {
    likes.value[id]--
    liked.value[id] = false
  } else {
    likes.value[id]++
    liked.value[id] = true
  }
}

// ── Kommentar-Logik ───────────────────────────────────
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

// ── Artikel öffnen / schließen ────────────────────────
function openArticle(article) {
  selectedArticle.value = article
  commentInput.value    = ''
  document.body.style.overflow = 'hidden'
}
function closeArticle() {
  selectedArticle.value = null
  document.body.style.overflow = ''
}

// ── Hilfsfunktionen ───────────────────────────────────
function formatDate(isoDate) {
  return new Date(isoDate).toLocaleDateString('de-AT', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  })
}

function formatDateLong() {
  return new Date().toLocaleDateString('de-AT', {
    weekday: 'long', day: 'numeric', month: 'long', year: 'numeric',
  })
}

function getPlaceholderImg(source, size) {
  const dims   = { featured: '860x420', card: '520x280', sidebar: '90x70', detail: '860x360' }
  const colors = { featured: 'c8e6c9/388e3c', card: 'e8f5e9/4caf50', sidebar: 'dcedc8/558b2f', detail: 'c8e6c9/2e7d32' }
  return `https://placehold.co/${dims[size]}/${colors[size]}?text=${encodeURIComponent(source)}`
}

function getTagForArticle(article) {
  if (article.score >= 90) return 'Top Story'
  if (article.score >= 80) return 'Empfohlen'
  if (article.score >= 70) return 'Lesenswert'
  return 'Aktuell'
}

const categories = [
  { icon: '🌍', name: 'Welt' },
  { icon: '🇦🇹', name: 'Österreich' },
  { icon: '🇩🇪', name: 'Deutschland' },
  { icon: '🇨🇭', name: 'Schweiz' },
  { icon: '💡', name: 'Innovation' },
  { icon: '❤️', name: 'Gesellschaft' },
]

const seeding = ref(false)
async function handleSeed() {
  seeding.value = true
  try {
    const msg  = await seedArticles()
    alert(msg)
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
      <div class="hp-spinner"></div>
      <p class="mt-3 text-muted">Lade Artikel…</p>
    </div>

    <!-- MAIN CONTENT -->
    <main v-else class="container py-4">

      <!-- Fallback-Hinweis -->
      <div v-if="error" class="alert alert-warning d-flex justify-content-between align-items-center mb-3">
        <span>⚠️ {{ error }}</span>
        <button class="btn btn-sm btn-outline-success" :disabled="seeding" @click="handleSeed">
          {{ seeding ? 'Seeding…' : '🌱 Seed Testdaten' }}
        </button>
      </div>

      <div class="row g-4">

        <!-- LEFT COLUMN -->
        <div class="col-lg-8">

          <!-- FEATURED ARTICLE -->
          <article v-if="featuredArticle" class="hp-featured mb-4">
            <div class="hp-featured-wrap" @click="openArticle(featuredArticle)" role="button" tabindex="0"
                 @keydown.enter="openArticle(featuredArticle)">
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
                  <span class="hp-dot">·</span>
                  <span>👍 {{ likes[featuredArticle.id] }}</span>
                </div>
              </div>
            </div>
          </article>

          <!-- SECTION HEADING -->
          <div class="hp-section-head mb-3">
            <h2 class="hp-section-title">Aktuelle Happy News</h2>
          </div>

          <!-- ARTICLE GRID -->
          <div class="row g-3">
            <div v-for="art in mainArticles" :key="art.id" class="col-sm-6 col-lg-4">
              <article class="hp-card h-100" @click="openArticle(art)" role="button" tabindex="0"
                       @keydown.enter="openArticle(art)">
                <img
                  :src="getPlaceholderImg(art.source, 'card')"
                  :alt="art.title"
                  class="hp-card-img"
                />
                <div class="hp-card-body">
                  <div class="mb-2">
                    <span class="hp-tag">{{ getTagForArticle(art) }}</span>
                    <span class="hp-tag hp-tag-outline ms-1">{{ art.source }}</span>
                  </div>
                  <h3 class="hp-card-title">{{ art.title }}</h3>
                  <div class="hp-meta mt-2">
                    <span>{{ formatDate(art.publishedAt) }}</span>
                    <span class="hp-dot">·</span>
                    <span>⭐ {{ art.score }}</span>
                    <span class="hp-dot">·</span>
                    <span>👍 {{ likes[art.id] ?? 0 }}</span>
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

          <!-- TOP SCORED (korrekt nach Score sortiert) -->
          <div class="hp-widget mb-4">
            <h5 class="hp-widget-title hp-widget-title-dark">🔥 Höchster Score</h5>
            <div class="hp-sidebar-list">
              <div
                v-for="(art, i) in sidebarArticles"
                :key="art.id"
                class="hp-sidebar-item"
                @click="openArticle(art)"
                role="button"
                tabindex="0"
                @keydown.enter="openArticle(art)"
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
              </div>
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

    <!-- ═══════════════════════════════════════════════ -->
    <!-- ARTIKEL-DETAIL MODAL                           -->
    <!-- ═══════════════════════════════════════════════ -->
    <Transition name="modal-fade">
      <div v-if="selectedArticle" class="hp-modal-backdrop" @click.self="closeArticle">
        <div class="hp-modal" role="dialog" aria-modal="true">

          <!-- Modal Header -->
          <div class="hp-modal-header">
            <div class="d-flex align-items-center gap-2">
              <span class="hp-tag">{{ getTagForArticle(selectedArticle) }}</span>
              <span class="hp-tag hp-tag-outline">{{ selectedArticle.source }}</span>
            </div>
            <button class="hp-modal-close" @click="closeArticle" aria-label="Schließen">✕</button>
          </div>

          <!-- Modal Body (scrollable) -->
          <div class="hp-modal-body">

            <!-- Hero Image -->
            <img
              :src="getPlaceholderImg(selectedArticle.source, 'detail')"
              :alt="selectedArticle.title"
              class="hp-modal-img"
            />

            <!-- Title & Meta -->
            <div class="hp-modal-content">
              <h2 class="hp-modal-title">{{ selectedArticle.title }}</h2>
              <div class="hp-meta mb-3">
                <span>📰 {{ selectedArticle.source }}</span>
                <span class="hp-dot">·</span>
                <span>{{ formatDate(selectedArticle.publishedAt) }}</span>
                <span class="hp-dot">·</span>
                <span>⭐ Score: {{ selectedArticle.score }}</span>
              </div>

              <!-- Summary / Artikel-Text -->
              <p class="hp-modal-summary">
                {{ selectedArticle.summary ?? 'Kein Kurztext verfügbar.' }}
              </p>

              <!-- Originallink -->
              <a :href="selectedArticle.url" target="_blank" rel="noopener" class="btn hp-btn-outline mb-4">
                🔗 Originalartikel öffnen
              </a>

              <!-- ─── LIKE / UPVOTE ─────────────────── -->
              <div class="hp-like-section">
                <button
                  class="hp-like-btn"
                  :class="{ 'hp-like-btn--active': liked[selectedArticle.id] }"
                  @click="toggleLike(selectedArticle.id)"
                >
                  <span class="hp-like-icon">{{ liked[selectedArticle.id] ? '👍' : '👍' }}</span>
                  <span class="hp-like-count">{{ likes[selectedArticle.id] ?? 0 }}</span>
                  <span class="hp-like-label">{{ liked[selectedArticle.id] ? 'Geliked!' : 'Gefällt mir' }}</span>
                </button>
                <span class="hp-like-hint">Hilf anderen, gute Nachrichten zu finden.</span>
              </div>

              <!-- ─── KOMMENTARE ────────────────────── -->
              <div class="hp-comments-section">
                <h4 class="hp-comments-heading">
                  💬 Kommentare
                  <span class="hp-comments-count">({{ (comments[selectedArticle.id] ?? []).length }})</span>
                </h4>

                <!-- Kommentar-Liste -->
                <div class="hp-comments-list">
                  <div
                    v-if="(comments[selectedArticle.id] ?? []).length === 0"
                    class="hp-comments-empty"
                  >
                    Noch keine Kommentare. Schreib den ersten! 🌱
                  </div>
                  <div
                    v-for="(c, idx) in (comments[selectedArticle.id] ?? [])"
                    :key="idx"
                    class="hp-comment"
                  >
                    <div class="hp-comment-avatar">{{ c.author[0] }}</div>
                    <div class="hp-comment-body">
                      <div class="hp-comment-meta">
                        <strong>{{ c.author }}</strong>
                        <span class="hp-comment-date">{{ c.date }}</span>
                      </div>
                      <p class="hp-comment-text">{{ c.text }}</p>
                    </div>
                  </div>
                </div>

                <!-- Kommentar schreiben -->
                <div class="hp-comment-form">
                  <textarea
                    v-model="commentInput"
                    class="hp-comment-input"
                    placeholder="Dein Kommentar…"
                    rows="3"
                    @keydown.ctrl.enter="submitComment(selectedArticle.id)"
                  ></textarea>
                  <div class="d-flex justify-content-between align-items-center mt-2">
                    <small class="text-muted">Ctrl + Enter zum Senden</small>
                    <button
                      class="btn hp-btn-sm"
                      :disabled="!commentInput.trim()"
                      @click="submitComment(selectedArticle.id)"
                    >
                      Kommentar senden ✉️
                    </button>
                  </div>
                </div>
              </div>

            </div><!-- /hp-modal-content -->
          </div><!-- /hp-modal-body -->
        </div><!-- /hp-modal -->
      </div><!-- /backdrop -->
    </Transition>

  </div>
</template>

<style scoped>
/* ── Modal Transitions ──────────────────────────────── */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.25s ease;
}
.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}
.modal-fade-enter-active .hp-modal,
.modal-fade-leave-active .hp-modal {
  transition: transform 0.25s ease;
}
.modal-fade-enter-from .hp-modal {
  transform: translateY(32px);
}

/* ── Backdrop ───────────────────────────────────────── */
.hp-modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  z-index: 1050;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  overflow-y: auto;
  padding: 2rem 1rem;
}

/* ── Modal Box ──────────────────────────────────────── */
.hp-modal {
  background: #fff;
  border-radius: 16px;
  width: 100%;
  max-width: 720px;
  box-shadow: 0 24px 64px rgba(0, 0, 0, 0.2);
  overflow: hidden;
  position: relative;
}

.hp-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid #e8f5e9;
  background: #f9fbe7;
}

.hp-modal-close {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: #555;
  line-height: 1;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  transition: background 0.15s;
}
.hp-modal-close:hover {
  background: #e8f5e9;
}

.hp-modal-body {
  max-height: calc(100vh - 8rem);
  overflow-y: auto;
}

.hp-modal-img {
  width: 100%;
  height: 240px;
  object-fit: cover;
  display: block;
}

.hp-modal-content {
  padding: 1.5rem;
}

.hp-modal-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1b5e20;
  line-height: 1.3;
  margin-bottom: 0.5rem;
}

.hp-modal-summary {
  font-size: 1rem;
  line-height: 1.7;
  color: #444;
  margin-bottom: 1.25rem;
}

/* ── Like Button ────────────────────────────────────── */
.hp-like-section {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.25rem;
  background: #f1f8e9;
  border-radius: 12px;
  margin-bottom: 2rem;
}

.hp-like-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.6rem 1.2rem;
  border: 2px solid #a5d6a7;
  background: #fff;
  border-radius: 999px;
  cursor: pointer;
  font-size: 0.95rem;
  font-weight: 600;
  color: #388e3c;
  transition: all 0.2s ease;
  user-select: none;
}
.hp-like-btn:hover {
  background: #e8f5e9;
  border-color: #66bb6a;
  transform: scale(1.04);
}
.hp-like-btn--active {
  background: #2e7d32;
  border-color: #2e7d32;
  color: #fff;
}
.hp-like-btn--active:hover {
  background: #388e3c;
}

.hp-like-icon  { font-size: 1.1rem; }
.hp-like-count { font-size: 1.1rem; }
.hp-like-label { font-size: 0.85rem; }
.hp-like-hint  { font-size: 0.8rem; color: #666; }

/* ── Comments ───────────────────────────────────────── */
.hp-comments-section {
  border-top: 2px solid #e8f5e9;
  padding-top: 1.5rem;
}

.hp-comments-heading {
  font-size: 1.1rem;
  font-weight: 700;
  color: #1b5e20;
  margin-bottom: 1rem;
}
.hp-comments-count {
  font-weight: 400;
  color: #888;
  font-size: 0.95rem;
}

.hp-comments-empty {
  text-align: center;
  color: #999;
  padding: 1.5rem;
  font-size: 0.95rem;
}

.hp-comments-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.hp-comment {
  display: flex;
  gap: 0.75rem;
}

.hp-comment-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: #a5d6a7;
  color: #1b5e20;
  font-weight: 700;
  font-size: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.hp-comment-body {
  background: #f9fbe7;
  border-radius: 10px;
  padding: 0.6rem 0.9rem;
  flex: 1;
}

.hp-comment-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.3rem;
  font-size: 0.85rem;
}
.hp-comment-date {
  color: #999;
  font-weight: 400;
}
.hp-comment-text {
  margin: 0;
  font-size: 0.93rem;
  color: #333;
  line-height: 1.5;
}

/* ── Comment Form ───────────────────────────────────── */
.hp-comment-form {
  border-top: 1px solid #e8f5e9;
  padding-top: 1rem;
}

.hp-comment-input {
  width: 100%;
  border: 1.5px solid #c8e6c9;
  border-radius: 10px;
  padding: 0.65rem 0.9rem;
  font-size: 0.93rem;
  resize: vertical;
  outline: none;
  transition: border-color 0.15s;
  font-family: inherit;
  color: #333;
  background: #fafffe;
}
.hp-comment-input:focus {
  border-color: #4caf50;
  box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.15);
}

/* ── Spinner ────────────────────────────────────────── */
.hp-spinner {
  width: 40px;
  height: 40px;
  margin: 0 auto;
  border: 4px solid #e8f5e9;
  border-top-color: #4caf50;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── Card & Featured: pointer cursor ───────────────── */
.hp-card,
.hp-featured-wrap,
.hp-sidebar-item {
  cursor: pointer;
}
.hp-card {
  transition: transform 0.15s, box-shadow 0.15s;
}
.hp-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(56, 142, 60, 0.15);
}
.hp-featured-wrap {
  transition: filter 0.15s;
}
.hp-featured-wrap:hover {
  filter: brightness(1.04);
}
</style>