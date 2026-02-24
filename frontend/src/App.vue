<script setup>
const articles = [
  {
    id: 1,
    title: "Technologie verändert Bildung",
    description: "Digitale Tools gewinnen immer mehr an Bedeutung in Schulen.",
    date: "24.02.2026",
  },
  {
    id: 2,
    title: "Wirtschaft wächst stärker als erwartet",
    description: "Neue Zahlen zeigen einen deutlichen Aufschwung im letzten Quartal.",
    date: "23.02.2026",
  },
  {
    id: 3,
    title: "KI revolutioniert IT Branche",
    description: "Automatisierung und Machine Learning verändern Geschäftsmodelle.",
    date: "22.02.2026",
  },
];
</script>

<template>
  <div class="container mt-5">
    <h1 class="mb-4 text-center">Zeitungsberichte</h1>

    <div class="row">
      <div class="col-md-4 mb-4" v-for="article in articles" :key="article.id">
        <div class="card h-100 shadow-sm">
          <div class="card-body">
            <h5 class="card-title">{{ article.title }}</h5>
            <p class="card-text">{{ article.description }}</p>
          </div>
          <div class="card-footer text-muted">
            {{ article.date }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
