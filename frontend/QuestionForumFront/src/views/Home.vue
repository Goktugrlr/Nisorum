<template>
  <div class="home">
    <header>
      <img :src="logo" alt="Site Logo" class="site-logo">
      <div class="header-content">
        <div v-if="currentUser" class="user-info">
          <span> 👤 <strong> {{ currentUser.Nickname }} </strong></span>
        </div>
        <button class="header-btn" @click="navigateToHome">Home</button>
        <button class="logout-btn" v-if="currentUser" @click="handleLogout">Logout</button>
        <button class="header-btn" v-if="!currentUser" @click="navigateToLogin">Login</button>
      </div>
    </header>

    <div class="filter-section">
      <div class="sort-options">
        <label for="sort-select"> <strong>Sort by:</strong></label>
        <select id="sort-select" v-model="selectedSort" @change="sortQuestions">
          <option value="desc">Most Votes First</option>
          <option value="asc">Fewest Votes First</option>
          <option value="dateDesc">Newest First</option>
          <option value="dateAsc">Oldest First</option>
        </select>
      </div>

      <button class="ask-btn" @click="checkLoginBeforeShowingModal">Ask Question</button>

      <div class="search-section">
        <input type="text" v-model="searchQuery" @input="filterQuestions" placeholder="Search questions..."/>
      </div>
    </div>

    <QuestionModal
      :isVisible="showModal"
      title="Enter Your Question"
      @update:visible="showModal = $event"
      @submit="handleSubmit"
    />

    <div v-if="filteredQuestions.length > 0" class="questions-list">
  <div v-for="question in filteredQuestions" :key="question.Id" class="question-item">
    <div class="vote-section">
      <span>{{ question.Vote }} Vote(s) </span>
      <span>{{ question.AnswerCount }} Answer(s)</span>
    </div>
    <div class="question-content">
      <h2><a :href="'/question/' + question.Id">{{ question.Title }}</a></h2>
      <p>{{ formatContent(question.Question_Content) }}</p>
      <div class="question-footer">
        <span>Author:<strong> {{ question.Nickname }}</strong></span>
        <span class="question-date"> <strong>{{ new Date(question.CreatedAt).toLocaleDateString() }} </strong></span>
      </div>
    </div>
  </div>
</div>

  </div>
</template>

<script>
import axios from 'axios';
import QuestionModal from '@/components/QuestionModal.vue';
import logo from '@/assets/nisorumlogo.png';

export default {
  name: 'Home',
  components: {
    QuestionModal,
  },
  data() {
    return {
      API_URL: 'http://localhost:27217/api/',
      showModal: false,
      questions: [],
      answers: [],
      users: {},
      currentUser: null,
      selectedSort: 'desc',
      searchQuery: '',
      filteredQuestions: [],
      logo,
    };
  },
  methods: {
    fetchQuestions() {
      axios.get(`${this.API_URL}Question`)
        .then((response) => {
          this.questions = response.data;
          this.filteredQuestions = this.questions; 
          this.sortQuestions();
          this.fetchUsers();
        })
        .catch((error) => {
          console.error("There was an error fetching the questions: ", error);
        });
    },
    fetchUsers() {
      axios.get(`${this.API_URL}user`)
        .then((response) => {
          const users = response.data;
          this.users = users.reduce((acc, user) => {
            acc[user.Id] = user;
            return acc;
          }, {});
          this.updateQuestionsWithUserInfo();
        })
        .catch((error) => {
          console.error("There was an error fetching the users: ", error);
        });
    },
    updateQuestionsWithUserInfo() {
      this.questions = this.questions.map(question => {
        const user = this.users[question.UserId];
        return {
          ...question,
          Nickname: user ? user.Nickname : 'Unknown'
        };
      });
      this.filteredQuestions = this.questions; 
    },

    fetchAnswers() {
    axios.get(`${this.API_URL}answer`)
      .then((response) => {
        this.answers = response.data;
        this.updateQuestionsWithAnswerCounts();
      })
      .catch((error) => {
        console.error("There was an error fetching the answers: ", error);
      });
  },
  updateQuestionsWithAnswerCounts() {
  const answerCounts = this.answers.reduce((acc, answer) => {
    acc[answer.QuestionId] = (acc[answer.QuestionId] || 0) + 1;
    return acc;
  }, {});

  this.questions = this.questions.map(question => {
    return {
      ...question,
      AnswerCount: answerCounts[question.Id] || 0, 
    };
  });
  this.filteredQuestions = this.questions; 
},
    handleSubmit(form) {
      const newQuestion = {
        Title: form.title,
        Question_Content: form.content,
        UserId: this.currentUser ? this.currentUser.Id : null,
      };

      axios.post(`${this.API_URL}Question`, newQuestion)
        .then(() => {
          this.fetchQuestions();
          this.showModal = false;
        })
        .catch((error) => {
          console.error("There was an error submitting the question: ", error);
        });
    },
    sortQuestions() {
      if (this.selectedSort === 'desc') {
        this.filteredQuestions.sort((a, b) => b.Vote - a.Vote);
      } else if (this.selectedSort === 'asc') {
        this.filteredQuestions.sort((a, b) => a.Vote - b.Vote);
      } else if (this.selectedSort === 'dateDesc') {
        this.filteredQuestions.sort((a, b) => new Date(b.CreatedAt) - new Date(a.CreatedAt));
      } else if (this.selectedSort === 'dateAsc') {
        this.filteredQuestions.sort((a, b) => new Date(a.CreatedAt) - new Date(b.CreatedAt));
      }
    },
    filterQuestions() {
      const query = this.searchQuery.toLowerCase();
      this.filteredQuestions = this.questions.filter(question =>
        question.Title.toLowerCase().includes(query) || question.Question_Content.toLowerCase().includes(query)
      );
      this.sortQuestions(); 
    },
    navigateToHome() {
      this.$router.push('/');
    },
    navigateToLogin() {
      this.$router.push('/login');
    },
    handleLogout() {
      localStorage.removeItem('user');
      this.currentUser = null;
      this.$router.push('/login');
    },
    checkLoginBeforeShowingModal() {
      if (this.currentUser) {
        this.showModal = true;
      } else {
        this.$router.push('/login');
      }
    },
    formatContent(content) {
      return content;
    },
  },
  mounted() {
    this.currentUser = JSON.parse(localStorage.getItem('user'));
    this.fetchQuestions();
    this.fetchAnswers();
    this.updateQuestionsWithAnswerCounts();
  },
}
</script>


<style scoped>
body {
  margin: 0;
  padding: 0;
  background-color: #FFFBE6; 
  font-family: Arial, sans-serif;
}

.home {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh; 
  background-color: #FFFBE6; 
  overflow-x: hidden;
}

header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 10px 20px;
  background-color: #FFFBE6; 
}

.site-logo {
  height: 80px; 
  width: auto; 
}

.header-content {
  display: flex;
  align-items: center;
}

.header-btn, .logout-btn {
  margin-left: 10px;
  padding: 10px 20px;
  font-size: 16px;
  background-color: #00712D; 
  color: #FFFBE6; 
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.header-btn:hover, .logout-btn:hover {
  background-color: #005B4F; 
}

.ask-btn {
  margin-bottom: 20px;
  padding: 10px 20px;
  font-size: 18px;
  background-color: #FF9100; 
  color: #FFFBE6;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.ask-btn:hover {
  background-color: #E67800; 
}

.user-info {
  display: flex;
  align-items: center;
}

.filter-section {
  display: flex;
  justify-content: space-between;
  width: 100%;
  max-width: 800px;
  margin-bottom: 20px;
}

.sort-options {
  display: flex;
  align-items: center;
}

.search-section {
  display: flex;
  align-items: center;
}

#sort-select {
  padding: 5px;
  font-size: 16px;
  border: 1px solid #D5ED9F;
  border-radius: 5px;
  background-color: #FFFBE6;
}

input[type="text"] {
  padding: 5px;
  font-size: 16px;
  border: 1px solid #D5ED9F;
  border-radius: 5px;
  background-color: #FFFBE6;
  margin-left: 20px;
}

input::placeholder {
  font-weight: bold;
  color: #2c2c2c; 
}

.questions-list {
  width: 100%;
  max-width: 800px;
}

.question-item {
  display: flex;
  align-items: flex-start;
  border: 1px solid #D5ED9F; 
  border-radius: 8px;
  padding: 10px;
  margin-bottom: 10px;
  background-color: #FFFBE6; 
  box-sizing: border-box; 
}

.vote-section {
  width: 120px; 
  display: flex;
  flex-direction: column; 
  align-items: center;
  justify-content: center; 
  font-size: 16px; 
  margin-right: 15px; 
  border-right: 1px solid #D5ED9F; 
  padding-right: 15px; 
  box-sizing: border-box; 
}

.vote-section span {
  display: inline-block; 
  margin: 0; 
  text-align: center; 
}

.vote-section  {
  margin-bottom: 5px; 
}


.question-content {
  flex: 1;
  text-align: left; 
}

.question-content h2 {
  margin: 0;
  color: #333333; 
}

.question-content a {
  text-decoration: none; 
  color: #333333;
}

.question-content a:hover {
  text-decoration: underline; 
}

.question-content p {
  margin: 5px 0 0;
  color: #2c2c2c;
  white-space: normal; 
  overflow: hidden; 
  text-overflow: ellipsis; 
  display: -webkit-box;
  -webkit-line-clamp: 2; 
  line-clamp: 2; 
  -webkit-box-orient: vertical; 
}


.question-footer {
  margin-top: 10px;
  font-size: 14px;
  color: #2c2c2c; 
  display: flex;
  justify-content: space-between;
}

.question-footer span {
  display: inline-block;
}

.question-date {
  margin-left: 10px;
}

</style>
