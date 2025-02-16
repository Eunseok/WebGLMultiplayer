using UnityEngine;

namespace Services
{
    public class UserService
    {
        private readonly FirebaseService _firebaseService;

        public UserService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public void AddUser(User user, string userId)
        {
            _firebaseService.Put($"users/{userId}", user);
            Debug.Log($"유저 추가: {user.name}");
        }

        public void GetUser(string userId)
        {
            _firebaseService.Get<User>($"users/{userId}",
                user => { Debug.Log($"유저 불러오기 성공: {user.name}, 나이: {user.age} "); },
                error => { Debug.LogError($"유저 불러오기 실패: {error.Message}"); });
        }
    }
}