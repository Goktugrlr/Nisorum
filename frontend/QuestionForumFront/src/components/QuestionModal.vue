<template>
  <div v-if="isVisible" class="modal-overlay" @click.self="closeModal">
    <div class="modal">
      <h1>{{ title }}</h1>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="title">Title</label>
          <input type="text" id="title" v-model="form.title" />
        </div>
        <div class="form-group">
          <label for="content">Content</label>
          <textarea id="content" v-model="form.content"></textarea>
        </div>
        <button type="submit" class="submit-btn">Submit</button>
        <button type="button" class="close-btn" @click="closeModal">Close</button>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: 'QuestionModal',
  props: {
    isVisible: {
      type: Boolean,
      required: true
    },
    title: {
      type: String,
      default: 'Modal Title'
    }
  },
  data() {
    return {
      form: {
        title: '',
        content: ''
      }
    };
  },
  watch: {
    isVisible(newVal) {
      if (newVal) {
        // Modal açıldığında formu sıfırla
        this.resetForm();
      }
    }
  },
  methods: {
    submitForm() {
      this.$emit('submit', this.form);
      this.closeModal();
    },
    closeModal() {
      this.$emit('update:visible', false);
    },
    resetForm() {
      this.form = {
        title: '',
        content: ''
      };
    }
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal {
  background: #FFFBE6; /* Açık Krem */
  border-radius: 8px;
  padding: 20px;
  width: 400px;
  max-width: 90%;
}

h1 {
  color: #00712D; /* Ana Renk */
  margin-top: 0;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  color: #00712D; /* Ana Renk */
}

.form-group input,
.form-group textarea {
  width: calc(100% - 22px); /* Adjusted to account for padding */
  padding: 10px;
  border: 1px solid #D5ED9F; /* Açık Yeşil */
  border-radius: 4px;
  color: #00712D; /* Ana Renk */
}

textarea {
  resize: none;
  min-height: 100px;
}

.submit-btn,
.close-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.submit-btn {
  background-color: #FF9100; /* Turuncu */
  color: #FFFBE6; /* Açık Krem */
  margin-right: 10px;
}

.close-btn {
  background-color: #00712D; /* Ana Renk */
  color: #FFFBE6; /* Açık Krem */
}
</style>
