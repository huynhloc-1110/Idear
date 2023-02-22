# Quy trình làm việc trên Git và GitHub
Dự án sẽ có 2 branch chính và những branch phụ:

## Những branch phụ
- Đây là những branch mà khi chúng ta tạo ra khi làm việc trên 1 tính năng cụ
thể.

> Ví dụ: Huy được giao làm login page cho project, thì Huy sẽ tạo 1 branch mới
tên là `feature-login` và làm việc trên branch này.

## `dev` branch
- Đây tượng trưng cho phiên bản hiện hành của project mà tất cả mọi người đang
làm việc trên đó. 1 branch phụ khi đã hoàn thành sẽ được merge vào `dev` branch.
Việc merge branch sẽ phải tiến hành qua pull request.

> Ví dụ: Huy đã hoàn thành xong `feature-login` branch và muốn merge nó vào
`dev` branch. Huy vào GitHub và tạo 1 một pull request cho `feature-login` merge
vào `dev`. Pull request phải được review ít nhất bởi 1 người khác (ví dụ Trí)
trước khi có thể tiến hành merge vào `dev`.

- Lưu ý rằng sau khi `dev` được `feature-login` merge vào, những branch phụ khác
phải được merge lại từ `dev`. Điều này đảm bảo các branch phụ khác sẽ được cập
nhật mới nhất từ `dev`.

> Ví dụ: Câu chuyện tiếp diễn sau khi Huy merge thành công `feature-login` vào
`dev`. Khoa lúc này cũng đang làm việc trên 1 branch phụ là `feature-purchase`,
Khoa nhận thấy pull request của Huy trước đó đã được merge thành công vào `dev`.
Do đó, Khoa cần cập nhật branch của mình từ `dev`. Khoa tiến hành merge `dev`
vào `feature-purchase` vả giải quyết các conflict nếu có.

## `main` branch
- Đây là default branch của github, tức là những người ghé thăm GitHub repos sẽ
thấy branch này đầu tiên và khi clone về sẽ hiện các file từ branch này trước.
Do đó, branch này được dùng cho các version release chính thức. Branch này sẽ
chỉ được merge từ `dev` branch sau khi xong hết các tính năng của 1 sprint.

> Ví dụ: Sprint này bao gồm 2 tính năng là `feature-login` và
`feature-purchase`. Khoa cũng đã hoàn thành xong `feature-purchase` và thành
công merge nó vào `dev`. Lúc này, `dev` đã hoàn thành 1 sprint và sẽ được merge
vào `main`.

# Các lệnh git cần thiết
- Chúng ta lấy project về máy bằng lệnh:
```shell
    git clone <repo_ssh_url>
```

- Tạo branch mới và đi tới branch đó:
```shell
    git checkout -b <ten_branch>
```

- Xóa 1 branch (chỉ dùng khi tạo nhầm hoặc xóa branch phụ đã
hoàn thành ở local):
```shell
    git checkout <branch_khac>      // đi tới branch khác
    git branch -d <branch_can_xoa>  // xóa branch cần xóa
```

- Push 1 branch lên GitHub:
```shell
    git push origin <ten_branch>
```

- Pull 1 branch phụ của người khác về:
```shell
    git checkout <branch_phu_muon_pull_ve>
    git pull origin <branch_phu_muon_pull_ve>
```

- Merge branch vào branch khác (chỉ dùng khi merge `dev` branch
vào branch phụ đang làm, còn muốn merge branch phụ vào `dev`
branch thì lên GitHub và bấm tạo pull request):
```shell
    git checkout dev                // đi tới `dev`
    git pull origin dev             // tại `dev`, pull dev mới về
    git checkout <feature_branch>   // đi tới branch phụ
    git merge dev                   // merge vào `dev` branch
```

## Lưu ý
Từ khóa `origin` chỉ remote repos của chúng ta trên GitHub. Ở ví dụ ngay bên
dưới, ở local có 2 branch: `develope` và `main`, trong đó `develop` là chỗ của
chúng ta hiện tại. Ở GitHub có 3 branch: `develop`, `feature-register` và
`main`, trong đó main là branch default trên GitHub.
```console
* develop
  main
  remotes/origin/HEAD -> origin/main
  remotes/origin/develop
  remotes/origin/feature-register
  remotes/origin/main
```
Như đã thấy, `feature-register` có trên remote nhưng vẫn chưa được đem về local.
Để lấy nó về để xem xét (hay để làm gì khác) thì dùng các lệnh:
```shell
    git checkout feature-register
    git pull origin feature-register
```

# Tham khảo
- Xem [video này](https://www.youtube.com/watch?v=1SXpE08hvGs) để hiểu về quy
trình GitFlow. Lưu ý: project của chúng ta không có `release` và `hotfix` branch
để tránh quá phức tạp.