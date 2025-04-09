<template>
  <main>
    <section>
      <div class="container">
        <div class="row">
          <div class="d-flex flex-column justify-content-center align-items-center image-container pt-5">
            <img
                class="img-fluid mt-1"
                loading="lazy"
                src="@/assets/images/page_not_found.svg"
                alt="Page Not Found"
                style="width: 350px; height: auto;"
            />
            <div class="text-center mt-4">
              <h1>PAGE NOT FOUND</h1>
              <p class="lead m-0">
                RETURN TO <router-link to="/login">HOME</router-link>
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
</template>

<script>
export default {};
</script>

<style scoped>
.image-container {
  min-height: 400px;
}
h1{
  color: #217bff;
  margin-top: 1.5rem;
  margin-bottom: 1rem;
}
p{
  font-size: 1rem;
}
p > a{
  text-decoration: none;
}
</style>
