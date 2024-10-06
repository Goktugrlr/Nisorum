<template>
  <div class="component-container">
    <div class="header">
      <div class="header-left">
        <img :src="logo" alt="Site Logo" class="site-logo"> 
      </div>
      <div class="header-right">
        <span v-if="isLoggedIn" class="nickname"><strong> 👤 {{ userNickname }} </strong></span>
        <button class="header-btn" @click="navigateToHome">Home</button>
        <button v-if="isLoggedIn" class="header-btn" @click="handleLogout">Logout</button>
        <button v-else class="header-btn" @click="navigateToLogin">Login</button>
      </div>
    </div>


    <div v-if="question" class="question-container">
      <div class="vote-section">
        <button
          class="vote-btn"
          :class="{ active: userVotes[question.Id] === 1 }"
          @click="updateVote('question', question.Id, 1)"
        >
          &#9650;
        </button>
        <p>{{ question.Vote }}</p>
        <button
          class="vote-btn"
          :class="{ active: userVotes[question.Id] === -1 }"
          @click="updateVote('question', question.Id, -1)"
        >
          &#9660;
        </button>
      </div>

      <div class="question-content">
        <h1>{{ question.Title }}</h1>
        <p>{{ question.Question_Content }}</p>
      </div>

      <div class="question-footer">
        <span class="author"> <strong>by: {{ userMap[question.UserId] || 'Unknown' }} </strong></span>
        <span class="created-date"> <br>{{ new Date(question.CreatedAt).toLocaleString() }} </br></span>
      </div>
    </div>

    <div class="sort-options">
      <label>
        <input type="radio" value="votes" v-model="sortOption"> Most Voted
      </label>
      <label>
        <input type="radio" value="leastVotes" v-model="sortOption"> Fewest Voted
      </label>
      <label>
        <input type="radio" value="newest" v-model="sortOption"> Newest
      </label>
      <label>
        <input type="radio" value="oldest" v-model="sortOption"> Oldest
      </label>
    </div>

    <div v-if="answers.length > 0" class="answers-list">
      <h2>Answers</h2>
      <div v-for="answer in sortedAnswers" :key="answer.Id" class="answer-item">
        <div class="vote-section">
          <button
            class="vote-btn"
            :class="{ active: userVotes[answer.Id] === 1 }"
            @click="updateVote('answer', answer.Id, 1)"
          >
            &#9650;
          </button>
          <p>{{ answer.Vote }}</p>
          <button
            class="vote-btn"
            :class="{ active: userVotes[answer.Id] === -1 }"
            @click="updateVote('answer', answer.Id, -1)"
          >
            &#9660;
          </button>
        </div>

        <div class="answer-content">
          <p>{{ answer.Answer_Content }}</p>
        </div>

        <div class="answer-header">
          <span class="answer-nickname">{{ answer.Nickname }}</span>
          <span class="answer-date"> <br>{{ new Date(answer.CreatedAt).toLocaleString() }} </br></span>
        </div>
        
      </div>
    </div>
    <div v-else class="no-answers">
      <h2>Answers</h2>
      <p>No answers yet, be the first to submit an answer.</p>
    </div>

    <div v-if="isLoggedIn" class="answer-section">
  <h2>Write Your Answer</h2>
  <textarea v-model="newAnswer" placeholder="Type your answer here..."></textarea>
  <button class="send-btn" @click="submitAnswer">Send Answer</button>
</div>
<div v-else class="login-prompt">
  <p>You need to <button @click="navigateToLogin" class="login-link">login</button> to answer.</p>
</div>
  </div>
</template>

<script>
import axios from 'axios';
import { useRouter } from 'vue-router';
import logo from '@/assets/nisorumlogo.png';

export default {
  name: 'QuestionDetail',
  data() {
    return {
      question: null,
      answers: [],
      newAnswer: '',
      API_URL: 'http://localhost:27217/api/',
      userNickname: '',
      isLoggedIn: false,
      sortOption: 'votes',
      userVotes: {}, 
      userMap: {},
      logo
    };
  },
  computed: {
    sortedAnswers() {
      let sorted = [...this.answers];

      if (this.sortOption === 'votes') {
        sorted.sort((a, b) => b.Vote - a.Vote);
      } else if (this.sortOption === 'leastVotes') {
        sorted.sort((a, b) => a.Vote - b.Vote);
      } else if (this.sortOption === 'newest') {
        sorted.sort((a, b) => new Date(b.CreatedAt) - new Date(a.CreatedAt));
      } else if (this.sortOption === 'oldest') {
        sorted.sort((a, b) => new Date(a.CreatedAt) - new Date(b.CreatedAt));
      }

      return sorted;
    }
  },
  methods: {
    fetchQuestion(id) {
      axios.get(`${this.API_URL}Question`)
        .then(response => {
          this.question = response.data.find(q => q.Id === parseInt(id));
          this.fetchAnswers(id);
        })
        .catch(error => {
          console.error("Error fetching questions:", error);
        });
    },
    fetchAnswers(questionId) {
      axios.get(`${this.API_URL}Answer`)
        .then(response => {
          this.answers = response.data.filter(a => a.QuestionId === parseInt(questionId));
          this.fetchUserNicknames(); 
        })
        .catch(error => {
          console.error("Error fetching answers:", error);
        });
    },
    fetchUserNicknames() {
      axios.get(`${this.API_URL}user`)
        .then(response => {
          response.data.forEach(user => {
            this.userMap[user.Id] = user.Nickname;
          });
          this.answers.forEach(answer => {
            answer.Nickname = this.userMap[answer.UserId] || 'Unknown'; 
          });
        })
        .catch(error => {
          console.error("Error fetching users:", error);
        });
    },
    fetchUserVotes() {
      if (!this.isLoggedIn) return;

      const userId = JSON.parse(localStorage.getItem('user')).Id;
      axios.get(`${this.API_URL}votes?UserId=${userId}`)
        .then(response => {
          this.userVotes = response.data.reduce((acc, vote) => {
            acc[vote.PostId] = vote.VoteValue;
            return acc;
          }, {});
        })
        .catch(error => {
          console.error("Error fetching user votes:", error);
        });
    },
    submitAnswer() {
      if (!this.newAnswer.trim()) {
        alert("Please enter an answer before submitting.");
        return;
      }

      const answerData = {
        QuestionId: this.question.Id,
        UserId: JSON.parse(localStorage.getItem('user')).Id, 
        Answer_Content: this.newAnswer
      };

      axios.post(`${this.API_URL}Answer`, answerData)
        .then(() => {
          this.fetchAnswers(this.question.Id); 
          this.newAnswer = ''; 
        })
        .catch(error => {
          console.error("Error submitting the answer:", error);
        });
    },

    updateVote(type, id, voteChange) {
  if (!this.isLoggedIn) {
    alert("Please login to vote.");
    return;
  }

  const userId = JSON.parse(localStorage.getItem('user')).Id;
  const existingVote = this.userVotes[id];

  
  if (existingVote !== undefined) {
    if (existingVote === voteChange) {
      const newVoteValue = 0; 
      this.userVotes[id] = newVoteValue; 

      const voteData = {
        UserId: userId,
        PostId: id,
        VoteType: type,
        VoteValue: newVoteValue
      };

      axios.delete(`${this.API_URL}Votes/${userId}/${id}/${type}`)
        .then(() => {
          if (type === 'question') {
            const updatedVote = this.question.Vote - existingVote; 
            axios.put(`${this.API_URL}Question`, { ...this.question, Vote: updatedVote });
            this.question.Vote = updatedVote;
          } else {
            const answerIndex = this.answers.findIndex(answer => answer.Id === id);
            const updatedVote = this.answers[answerIndex].Vote - existingVote; 
            axios.put(`${this.API_URL}Answer`, { ...this.answers[answerIndex], Vote: updatedVote });
            this.answers[answerIndex].Vote = updatedVote;
          }
        })
        .catch(error => {
          console.error("Error removing the vote:", error);
        });

      return; 
    } else {
      const changeAmount = 2 * (voteChange === 1 ? 1 : -1);
      this.userVotes[id] = voteChange; 
    }
  } else {
    this.userVotes[id] = voteChange; 
  }

  const voteData = {
    UserId: userId,
    PostId: id,
    VoteType: type,
    VoteValue: voteChange
  };

  axios.post(`${this.API_URL}votes`, voteData)
    .then(() => {
      if (type === 'question') {
        const updatedVote = this.question.Vote + (existingVote ? 2 : 1) * (voteChange === 1 ? 1 : -1);
        axios.put(`${this.API_URL}Question`, { ...this.question, Vote: updatedVote });
        this.question.Vote = updatedVote;
      } else {
        const answerIndex = this.answers.findIndex(answer => answer.Id === id);
        const updatedVote = this.answers[answerIndex].Vote + (existingVote ? 2 : 1) * (voteChange === 1 ? 1 : -1);
        axios.put(`${this.API_URL}Answer`, { ...this.answers[answerIndex], Vote: updatedVote });
        this.answers[answerIndex].Vote = updatedVote;
      }
    })
    .catch(error => {
      console.error("Error submitting the vote:", error);
    });
},

    navigateToLogin() {
      this.$router.push('/login');
    },
    navigateToHome() {
      this.$router.push('/');
    },
    handleLogout() {
      localStorage.removeItem('user');
      this.isLoggedIn = false;
      this.$router.push('/login');
    }
  },
  mounted() {
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.isLoggedIn = true;
      this.userNickname = user.Nickname;
    }

    const questionId = this.$route.params.id;
    this.fetchQuestion(questionId);
    this.fetchUserVotes();
  }
};
</script>


<style scoped>
.component-container {
  background-color: #FFFBE6; 
  min-height: 100vh; 
  padding: 20px; 
  box-sizing: border-box;
  overflow-x: hidden;
}

.header {
  display: flex;
  justify-content: space-between; 
  align-items: center;
  width: 100%;
  margin-bottom: 20px;
}

.header-left {
  display: flex;
  align-items: center;
}

.header-right {
  display: flex;
  align-items: center;
}

.site-logo {
  height: 80px; 
  width: auto; 
}

.nickname {
  margin-right: 10px;
  font-size: 16px;
}

.header-btn {
  margin-left: 10px;
  padding: 10px 20px;
  font-size: 16px;
  background-color: #00712D; 
  color: white; 
  border: none; 
  border-radius: 4px; 
  cursor: pointer; 
}

.header-btn:hover {
  background-color: #e68a00; 
}

.question-container, .answer-item {
  display: flex; 
  align-items: flex-start;
  margin-bottom: 20px;
}

.question-container {
  background-color: #D5ED9F; 
  padding: 20px; 
  border-radius: 8px;
  margin-bottom: 20px; 
}

.vote-section {
  margin-right: 15px;
  text-align: center;
  width: 70px; 
}

.vote-section p {
  font-size: 24px;
  font-weight: bold;
}

.vote-btn {
  background-color: #FF9100; 
  color: #D5ED9F; 
  border: none;
  font-size: 24px;
  cursor: pointer;
  padding: 5px; 
  border-radius: 5px; 
}

.vote-btn:hover {
  background-color: #FFFBE6; 
}

.vote-btn.active {
  background-color: #00712D;
  font-weight: bold;
}

.question-content {
  flex: 1; 
  text-align: left;
}

.question-content h1 {
  color: #333333; 
}

.question-content p {
  color: #333333; 
}

.answer-content p {
  color: #333333;
  text-align: left;
}


.answer-section {
  margin-top: 20px;
  padding: 10px;
  background-color: #00712D; 
  color: #D5ED9F; 
  box-sizing: border-box; 
}

textarea {
  width: calc(100% - 22px); 
  height: 100px;
  padding: 10px;
  border-radius: 4px;
  background-color: #D5ED9F; 
  color: #333333; 
  box-sizing: border-box; 
  resize: none;
}

.send-btn {
  padding: 10px 20px;
  font-size: 16px;
  background-color: #FF9100; 
  color: white; 
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.send-btn:hover {
  background-color: #e68000; 
}

.answers-list {
  margin-top: 20px;
  background-color: #D5ED9F; 
  padding: 10px; 
  border-radius: 8px; 
}

.answers-list h2 {
  color: #333333; 
}

.answer-item {
  display: flex;
  align-items: flex-start; 
  margin-bottom: 20px;
  padding: 10px;
  background-color: #D5ED9F; 
  border: 1px solid #00712D;
  border-radius: 8px;
}

.answer-content {
  flex: 1; 
  margin-left: 15px;
}

.answer-header {
  margin-top: 110px;
}

.answer-nickname {
  font-weight: bold;
  font-size: 16px;
  color: #333333;
  text-align: left;
}

.answer-date {
  font-size: 12px;
  color: #333333;
  text-align: right;
}

.sort-options {
  margin-bottom: 20px;
  background-color: #D5ED9F; 
  padding: 10px; 
  border-radius: 8px; 
}


.sort-options label {
  color: #00712D; 
  font-size: 16px; 
}

.sort-options input[type="radio"] {
  accent-color: #00712D; 
}

.question-footer {
  display: flex;
  flex-direction: column; 
  align-items: flex-end; 
  margin-top: 100px;
  font-size: 14px;
  color: #333333;
}

.author {
  text-align: right;
}

.created-date {
  text-align: right; 
}

.no-answers {
  background-color: #D5ED9F; 
  padding: 20px;
  border-radius: 8px;
  text-align: center;
  font-size: 18px;
  color: #00712D; 
}

.login-prompt {
  background-color: #f8d7da; 
  color: #721c24; 
  padding: 15px;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  margin-top: 20px;
}

.login-link {
  background: none;
  border: none;
  color: #00712D; 
  cursor: pointer;
  text-decoration: underline;
}

.login-link:hover {
  color: navy; 
}

</style>
