<template>
  <div class="signup">
    <div class="header">
      <img src="@/assets/nisorumlogo.png" alt="Site Logo" class="site-logo" />
      <button class="header-btn" @click="navigateToHome">Home</button>
    </div>
    <h1>Sign Up</h1>
    <form @submit.prevent="handleSignUp">
      <div>
        <label for="nickname">Nickname:</label>
        <input type="text" id="nickname" v-model="nickname" required />
      </div>
      <div>
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="email" required />
      </div>
      <div>
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" required />
      </div>
      <div>
        <label for="confirmPassword">Confirm Password:</label>
        <input type="password" id="confirmPassword" v-model="confirmPassword" required />
        <p v-if="passwordMismatch" class="error-message">Passwords do not match!</p>
      </div>
      <button type="submit" :disabled="passwordMismatch">Sign Up</button>
    </form>
    <p>Already have an account? <button @click="navigateToLogin">Login</button></p>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'SignUp',
  data() {
    return {
      nickname: '',
      email: '',
      password: '',
      confirmPassword: ''
    };
  },
  computed: {
    passwordMismatch() {
      return this.password && this.confirmPassword && this.password !== this.confirmPassword;
    }
  },
  methods: {
    handleSignUp() {
      if (this.passwordMismatch) {
        console.log("Passwords do not match");
        return;
      }

      const newUser = {
        Nickname: this.nickname,
        Email: this.email,
        Password: this.password
      };

      axios.post('http://localhost:27217/api/User', newUser)
    .then(() => {
      console.log("Sign Up successful");
      this.$router.push('/login');
    })
    .catch((error) => {
      console.error("There was an error signing up: ", error);
      console.error("Error response data: ", error.response?.data);
    });
    },
    navigateToLogin() {
      this.$router.push('/login');
    },
    navigateToHome() {
      this.$router.push('/');
    }
  }
}
</script>

<style scoped>
html, body {
  margin: 0;
  padding: 0;
  height: 100%;
  background-color: #FFFBE6;
  overflow: hidden;
  font-family: Arial, sans-serif;
}

.site-logo {
  height: 80px;
  width: auto;
}

.signup {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start; /* İçeriği yukarı taşı */
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  margin-bottom: 10px;
}

.header-btn {
  margin-left: 10px;
  padding: 10px 20px;
  font-size: 16px;
  background-color: #00712D;
  color: #FFFBE6;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.header-btn:hover {
  background-color: #005B4F;
}

h1 {
  color: #00712D;
  margin-bottom: 15px;
}

form {
  display: flex;
  flex-direction: column;
  width: 300px;
  background-color: #D5ED9F;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

div {
  margin-bottom: 10px;
}

label {
  color: #00712D;
  display: block;
  margin-bottom: 5px;
}

input {
  width: 100%;
  padding: 8px;
  border: 1px solid #00712D;
  border-radius: 4px;
  box-sizing: border-box;
}

button {
  margin-top: 10px;
  padding: 10px;
  background-color: #FF9100;
  color: #FFFBE6;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #FF6F00;
}

p {
  margin-top: 5px;
}

p button {
  background-color: #FF9100;
  color: #FFFBE6;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

p button:hover {
  background-color: #FF6F00;
}

.error-message {
  color: red;
  font-size: 12px;
}
</style>
