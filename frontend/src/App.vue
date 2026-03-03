<script setup>
const featuredArticle = {
  tag: 'Ernährung',
  title: 'Die 10 gesündesten Snacks für Kinder unter 5 Jahren',
  excerpt: 'Kleine Mägen, großer Hunger – wir zeigen, welche Snacks wirklich satt machen und dabei gesund sind. Von Obst bis Gemüse: Diese Ideen sind schnell gemacht und bei Kindern beliebt.',
  author: 'Dr. Anna Müller',
  date: '03.03.2026',
  img: 'https://placehold.co/860x420/c8e6c9/388e3c?text=Featured+Artikel',
}

const mainArticles = [
  {
    tag: 'Gesundheit',
    tag2: 'Ratgeber',
    title: 'Impfkalender 2026: Was Eltern jetzt wissen müssen',
    excerpt: 'Die STIKO hat neue Empfehlungen herausgegeben. Wir erklären, welche Impfungen wann fällig sind.',
    author: 'Dr. Markus Weber',
    date: '02.03.2026',
    img: 'https://placehold.co/520x280/e8f5e9/4caf50?text=Gesundheit',
  },
  {
    tag: 'Bildung',
    tag2: 'Schule',
    title: 'Lernen mit Spaß: Die besten Lern-Apps 2026 im Test',
    excerpt: 'Wir haben 15 Lern-Apps unter die Lupe genommen und verraten, welche wirklich fördern.',
    author: 'Sandra Klein',
    date: '01.03.2026',
    img: 'https://placehold.co/520x280/e8f5e9/4caf50?text=Bildung',
  },
  {
    tag: 'Schlaf',
    tag2: 'Baby',
    title: 'Einschlafroutinen: So schläft dein Baby durch die Nacht',
    excerpt: 'Schlafmangel ist für viele Eltern das größte Problem. Expertin erklärt, was wirklich hilft.',
    author: 'Laura Bauer',
    date: '28.02.2026',
    img: 'https://placehold.co/520x280/e8f5e9/4caf50?text=Schlaf',
  },
]

const sidebarArticles = [
  { tag: 'Spielzeug', title: 'Die 5 sichersten Spielzeuge für Kleinkinder', date: '03.03.2026', img: 'https://placehold.co/90x70/e8f5e9/4caf50?text=🧸' },
  { tag: 'Aktivität', title: 'Bewegung im Winter: Ideen für drinnen', date: '02.03.2026', img: 'https://placehold.co/90x70/e8f5e9/4caf50?text=🏃' },
  { tag: 'Ernährung', title: 'Zuckerfrei kochen für die ganze Familie', date: '01.03.2026', img: 'https://placehold.co/90x70/e8f5e9/4caf50?text=🍎' },
  { tag: 'Gesundheit', title: 'Wann zum Arzt? Die wichtigsten Warnsignale', date: '28.02.2026', img: 'https://placehold.co/90x70/e8f5e9/4caf50?text=❤️' },
]

const categories = [
  { icon: '🍎', name: 'Ernährung' },
  { icon: '🧸', name: 'Spielzeug' },
  { icon: '📚', name: 'Bildung' },
  { icon: '💤', name: 'Schlaf' },
  { icon: '🏃', name: 'Aktivität' },
  { icon: '❤️', name: 'Gesundheit' },
]
</script>

<template>
  <div class="hp-app">

    <!-- TOP BAR -->
    <div class="hp-topbar">
      <div class="container d-flex justify-content-between align-items-center">
        <span>Deine Plattform für Happy Nachrichten aus der gesamten Welt!</span>
        <span>Dienstag, 3. März 2026</span>
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

    <!-- MAIN CONTENT -->
    <main class="container py-4">
      <div class="row g-4">

        <!-- LEFT COLUMN -->
        <div class="col-lg-8">

          <!-- FEATURED ARTICLE -->
          <article class="hp-featured mb-4">
            <div class="hp-featured-wrap">
              <img :src="featuredArticle.img" :alt="featuredArticle.title" class="hp-featured-img" />
              <div class="hp-featured-overlay">
                <span class="hp-tag hp-tag-white mb-2 d-inline-block">{{ featuredArticle.tag }}</span>
                <h1 class="hp-featured-title">{{ featuredArticle.title }}</h1>
                <p class="hp-featured-excerpt d-none d-md-block">{{ featuredArticle.excerpt }}</p>
                <div class="hp-meta">
                  <span>✍️ {{ featuredArticle.author }}</span>
                  <span class="hp-dot">·</span>
                  <span>{{ featuredArticle.date }}</span>
                </div>
              </div>
            </div>
          </article>

          <!-- SECTION HEADING -->
          <div class="hp-section-head mb-3">
            <h2 class="hp-section-title">Aktuelle Ratgeber</h2>
          </div>

          <!-- ARTICLE GRID -->
          <div class="row g-3">
            <div v-for="art in mainArticles" :key="art.title" class="col-sm-6 col-lg-4">
              <article class="hp-card h-100">
                <a href="#"><img :src="art.img" :alt="art.title" class="hp-card-img" /></a>
                <div class="hp-card-body">
                  <div class="mb-2">
                    <span class="hp-tag">{{ art.tag }}</span>
                    <span class="hp-tag hp-tag-outline ms-1">{{ art.tag2 }}</span>
                  </div>
                  <h3 class="hp-card-title"><a href="#">{{ art.title }}</a></h3>
                  <p class="hp-card-excerpt">{{ art.excerpt }}</p>
                  <div class="hp-meta mt-2">
                    <span>{{ art.author }}</span>
                    <span class="hp-dot">·</span>
                    <span>{{ art.date }}</span>
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
            <p class="hp-widget-text">Wöchentlich die besten Tipps direkt ins Postfach.</p>
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

          <!-- MEIST GELESEN -->
          <div class="hp-widget mb-4">
            <h5 class="hp-widget-title hp-widget-title-dark">🔥 Meist gelesen</h5>
            <div class="hp-sidebar-list">
              <a v-for="(art, i) in sidebarArticles" :key="art.title" href="#" class="hp-sidebar-item">
                <span class="hp-sidebar-num">{{ i + 1 }}</span>
                <img :src="art.img" :alt="art.title" class="hp-sidebar-img" />
                <div class="hp-sidebar-info">
                  <span class="hp-tag hp-tag-xs">{{ art.tag }}</span>
                  <p class="hp-sidebar-title">{{ art.title }}</p>
                  <small class="hp-sidebar-date">{{ art.date }}</small>
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
            <p class="hp-footer-text mt-2">Dein verlässlicher Ratgeber rund um Kinder, Gesundheit und Familie.</p>
          </div>
          <div class="col-6 col-lg-2">
            <h6 class="hp-footer-heading">Themen</h6>
            <ul class="list-unstyled hp-footer-links">
              <li><a href="#">Ernährung</a></li>
              <li><a href="#">Gesundheit</a></li>
              <li><a href="#">Bildung</a></li>
              <li><a href="#">Schlaf</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <h6 class="hp-footer-heading">Über uns</h6>
            <ul class="list-unstyled hp-footer-links">
              <li><a href="#">Team</a></li>
              <li><a href="#">Kontakt</a></li>
              <li><a href="#">Karriere</a></li>
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