<template>
  <div class="login">
    <div class="header">
      <img src="@/assets/nisorumlogo.png" alt="Site Logo" class="site-logo" />
      <button class="header-btn" @click="navigateToHome">Home</button>
    </div>
    <h1>Login</h1>
    <form @submit.prevent="handleLogin">
      <div class="input-group">
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="email" required />
      </div>
      <div class="input-group password-group">
        <label for="password">Password:</label>
        <div class="password-container">
          <input 
            :type="showPassword ? 'text' : 'password'" 
            id="password" 
            v-model="password" 
            required 
          />
          <button type="button" @click="togglePassword" class="password-toggle">
            <span v-if="showPassword">🔒</span>
            <span v-else>👁️</span>
          </button>
        </div>
      </div>
      <button type="submit">Login</button>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
    </form>
    <p>Don't have an account? <button @click="navigateToSignUp">Sign up</button></p>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'Login',
  data() {
    return {
      email: '',
      password: '',
      errorMessage: '',
      showPassword: false
    };
  },
  methods: {
    handleLogin() {
      this.errorMessage = '';
      axios.get('http://localhost:27217/api/User')
        .then((response) => {
          const user = response.data.find(user => user.Email === this.email && user.Password === this.password);
          if (user) {
            localStorage.setItem('user', JSON.stringify(user));
            this.$router.push('/');
          } else {
            this.errorMessage = "Invalid email or password";
          }
        })
        .catch((error) => {
          console.error("There was an error logging in: ", error);
          this.errorMessage = "There was an error logging in";
        });
    },
    navigateToSignUp() {
      this.$router.push('/signup');
    },
    navigateToHome() {
      this.$router.push('/');
    },
    togglePassword() {
      this.showPassword = !this.showPassword;
    }
  }
}
</script>

<style scoped>
body {
  margin: 0;
  padding: 0;
  background-color: #FFFBE6;
  font-family: Arial, sans-serif;
}

.login {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start;
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

.input-group {
  margin-bottom: 10px; 
}

.password-group {
  position: relative;
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

.password-container {
  position: relative;
}

.password-toggle {
  position: absolute;
  top: 5%;
  right: 5px;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  color: #00712D;
  font-size: 14px;
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
  margin-top: 5px;
}

.site-logo {
  height: 80px;
  width: auto;  
}
</style>
